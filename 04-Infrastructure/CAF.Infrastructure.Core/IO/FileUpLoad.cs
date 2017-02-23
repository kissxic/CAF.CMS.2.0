using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace CAF.Infrastructure.Core.IO
{
    public class FileUpLoad
    {
        /// <summary>
        /// 文件上传方法
        /// </summary>
        /// <param name="postedFile">文件流</param>
        /// <param name="saveDir">上传目录名称</param>
        /// <returns>上传后文件信息</returns>
        public static string FileSaveAs(PostedFileResult postedFile,string saveDir)
        {
            try
            {
                var webHelper = EngineContext.Current.Resolve<IWebHelper>();
               
                int fileSize = postedFile.Size; //获得文件大小，以字节为单位
                string fileName = postedFile.FileName; //取得文件名有后缀
                string upLoadPath = GetUpLoadPath(saveDir); //上传目录相对路径
                string fullUpLoadPath = webHelper.MapPath(upLoadPath); //上传目录的物理路径
                string newFilePath = upLoadPath + postedFile.FileName ; //上传后的路径

                //检查文件扩展名是否合法
                //if (!CheckFileExt(fileExt))
                //{
                //    return "{\"status\": 0, \"msg\": \"不允许上传" + fileExt + "类型的文件！\"}";
                //}
                ////检查文件大小是否合法
                //if (!CheckFileSize(fileExt, fileSize))
                //{
                //    return "{\"status\": 0, \"msg\": \"文件超过限制的大小！\"}";
                //}
                //检查上传的物理路径是否存在，不存在则创建
                if (!Directory.Exists(fullUpLoadPath))
                {
                    Directory.CreateDirectory(fullUpLoadPath);
                }

                //保存文件
                postedFile.File.SaveAs(fullUpLoadPath + fileName);
                return newFilePath;
            }
            catch
            {
                return "";
            }
        }


        /// <summary>
        /// this function will take the ImagePath and crop it returns the bytes array
        /// </summary>
        /// <param name="imagePath">Physicall Image Path</param>
        /// <param name="Width">Widht of the Image</param>
        /// <param name="Height">Height of the Image</param>
        /// <param name="X">Start Croping from Left</param>
        /// <param name="Y">Start Croping from Top</param>
        /// <returns></returns>
        public static byte[] CropImages(string imagePath, int Width, int Height, int X, int Y)
        {

            //get Image from Path
            using (Image orignalImage = Image.FromFile(imagePath))
            {
                ImageFormat format = orignalImage.RawFormat;
                using (MemoryStream ms = new MemoryStream())
                {
                    if (format.Equals(ImageFormat.Jpeg))
                    {
                        orignalImage.Save(ms, ImageFormat.Jpeg);
                    }
                    else if (format.Equals(ImageFormat.Png))
                    {
                        orignalImage.Save(ms, ImageFormat.Png);
                    }
                    else if (format.Equals(ImageFormat.Bmp))
                    {
                        orignalImage.Save(ms, ImageFormat.Bmp);
                    }
                    else if (format.Equals(ImageFormat.Gif))
                    {
                        orignalImage.Save(ms, ImageFormat.Gif);
                    }
                    else if (format.Equals(ImageFormat.Icon))
                    {
                        orignalImage.Save(ms, ImageFormat.Icon);
                    }
                    byte[] buffer = new byte[ms.Length];
                    //Image.Save()会改变MemoryStream的Position，需要重新Seek到Begin
                    ms.Seek(0, SeekOrigin.Begin);
                    ms.Read(buffer, 0, buffer.Length);
                  return  CropImages(buffer, Width, Height, X, Y);
                }
               

            }



        }

        public static byte[] CropImages(byte[] buffer, int Width, int Height, int X, int Y)
        {
            MemoryStream ms = new MemoryStream(buffer);
            //get Image from Path
            using (Image orignalImage = System.Drawing.Image.FromStream(ms))
            {
                //Create a bitmap as well.
                using (Bitmap bitmap = new Bitmap(Width, Height))
                {
                    //set the resolution of the bitmap
                    bitmap.SetResolution(orignalImage.HorizontalResolution, orignalImage.VerticalResolution);

                    //get Graphics
                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {

                        graphics.SmoothingMode = SmoothingMode.AntiAlias;

                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

                        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        //Draw image from orignalImage to Bitmap
                        graphics.DrawImage(orignalImage, new Rectangle(0, 0, Width, Height), X, Y, Width, Height, GraphicsUnit.Pixel);

                        MemoryStream imageStream = new MemoryStream();
                        //save bitmap into memory stream
                        bitmap.Save(imageStream, orignalImage.RawFormat);
                        //return byte buffer
                        return imageStream.GetBuffer();
                    }

                }

            }



        }

        public static byte[] DrawText(String text, Font font, Color textColor, Color backColor)
        {
            //first, create a dummy bitmap just to get a graphics object
            Image image = new Bitmap(1, 1);
            Graphics drawing = Graphics.FromImage(image);

            //measure the string to see how big the image needs to be
            SizeF textSize = drawing.MeasureString(text, font);

            //free up the dummy image and old graphics object
            image.Dispose();
            drawing.Dispose();

            //create a new image of the right size
            image = new Bitmap((int)textSize.Width, (int)textSize.Height);

            drawing = Graphics.FromImage(image);

            //paint the background
            drawing.Clear(backColor);

            //create a brush for the text
            Brush textBrush = new SolidBrush(textColor);

            drawing.DrawString(text, font, textBrush, 0, 0);

            drawing.Save();

            textBrush.Dispose();
            drawing.Dispose();

            MemoryStream ms = new MemoryStream();
            image.Save(ms, ImageFormat.Jpeg);

            return ms.GetBuffer();


        }
        #region 私有方法
        /// <summary>
        /// 返回上传目录相对路径
        /// </summary>
        /// <param name="fileName">上传文件名</param>
        private static string GetUpLoadPath(string folderName)
        {
            var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            string path = "/Media/Files/{0}/".FormatCurrent(folderName); //站点目录+上传目录

            //按年月/日存入不同的文件夹
            path += DateTime.Now.ToString("yyyyMM") + "/" + DateTime.Now.ToString("dd");

            return path + "/";
        }


        #endregion

    }
}
