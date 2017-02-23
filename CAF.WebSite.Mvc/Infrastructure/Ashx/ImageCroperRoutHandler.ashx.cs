using CAF.Infrastructure.Core.IO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace CAF.WebSite.Mvc.Infrastructure.Ashx
{
    /// <summary>
    /// 创建一个通用的处理程序将处理所有的图像剪辑
    /// </summary>
    public class ImageCroperRoutHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            ImageCroperHttpHandler imageCroperHandler = new ImageCroperHttpHandler();
            return imageCroperHandler;
        }
        /// <summary>
        /// Summary description for ImageCroperHandler
        /// </summary>
        private class ImageCroperHttpHandler : IHttpHandler
        {


            public bool IsReusable
            {
                get
                {
                    return false;
                }
            }
            public void ProcessRequest(HttpContext context)
            {

                try
                {
                    //Get Rout Parameters from Request
                    var routeValues = context.Request.RequestContext.RouteData.Values;

                    //First of All get the Image Name from the URL
                    string imagePath = context.Request.RequestContext.RouteData.Values["imgName"].ToString();

                    //get the physicall path of the images which are stored in Images Directory located at the root of the application
                    imagePath = Path.Combine(HttpContext.Current.Server.MapPath("~/Media/Thumbs/0000"), imagePath);

                    //Get the Width Parameter from URL
                    var temp = context.Request.RequestContext.RouteData.Values["w"].ToString();
                    //parse Width of image 
                    int w = int.Parse(temp);

                    //Get the Height Parameter from URL
                    temp = context.Request.RequestContext.RouteData.Values["h"].ToString();
                    int h = int.Parse(temp);

                    temp = context.Request.RequestContext.RouteData.Values["x"].ToString();
                    int x = int.Parse(temp);

                    temp = context.Request.RequestContext.RouteData.Values["y"].ToString();
                    int y = int.Parse(temp);

                    //Now Crop the image and get Bytes Array
                    byte[] CropImage = FileUpLoad.CropImages(imagePath, w, h, x, y);

                    //set the Content Type to Images
                    context.Response.ContentType = "image/jpeg";

                    //Now Write the Bytes in Response
                    context.Response.BinaryWrite(CropImage);
                    //End the response
                    context.Response.End();
                }
                catch (Exception exception)
                {
                    //if there is an error it will return you image with exception message
                    //you can handle it by your way
                    context.Response.ContentType = "image/jpeg";
                    Font font = new Font("Arial", 12);
                    byte[] img = FileUpLoad.DrawText(exception.Message, font, Color.White, Color.Black);
                    context.Response.BinaryWrite(img);
                    context.Response.End();
                }
            }
          

        }
    }
}