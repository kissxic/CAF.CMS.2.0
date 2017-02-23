using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;

namespace CAF.Infrastructure.Core.Utilities
{
    public class CaptchaImage
    {
        /// <summary>
        /// Amount of random font warping to apply to rendered text
        /// </summary>
        public enum FontWarpFactor
        {
            None,
            Low,
            Medium,
            High,
            Extreme
        }
        /// <summary>
        /// Amount of background noise to add to rendered image
        /// </summary>
        public enum BackgroundNoiseLevel
        {
            None,
            Low,
            Medium,
            High,
            Extreme
        }
        /// <summary>
        /// Amount of curved line noise to add to rendered image
        /// </summary>
        public enum LineNoiseLevel
        {
            None,
            Low,
            Medium,
            High,
            Extreme
        }
        private int _height;
        private int _width;
        private Random _rand;
        private DateTime _generatedAt;
        private string _randomText;
        private int _randomTextLength;
        private string _randomTextChars;
        private string _fontFamilyName;
        private CaptchaImage.FontWarpFactor _fontWarp;
        private CaptchaImage.BackgroundNoiseLevel _backgroundNoise;
        private CaptchaImage.LineNoiseLevel _lineNoise;
        private string _guid;
        private string _fontWhitelist;
        private Color _backColor = Color.White;
        private Color _fontColor = Color.Black;
        private Color _noiseColor = Color.Black;
        private Color _lineColor = Color.Black;
        /// <summary>
        /// Returns a GUID that uniquely identifies this Captcha
        /// </summary>
        public string UniqueId
        {
            get
            {
                return this._guid;
            }
        }
        /// <summary>
        /// Returns the date and time this image was last rendered
        /// </summary>
        public DateTime RenderedAt
        {
            get
            {
                return this._generatedAt;
            }
        }
        /// <summary>
        /// Font family to use when drawing the Captcha text. If no font is provided, a random font will be chosen from the font whitelist for each character.
        /// </summary>
        public string Font
        {
            get
            {
                return this._fontFamilyName;
            }
            set
            {
                Font font = null;
                try
                {
                    font = new Font(value, 12f);
                    this._fontFamilyName = value;
                }
                catch (Exception)
                {
                    this._fontFamilyName = FontFamily.GenericSerif.Name;
                }
                finally
                {
                    font.Dispose();
                }
            }
        }
        /// <summary>
        /// Amount of random warping to apply to the Captcha text.
        /// </summary>
        public CaptchaImage.FontWarpFactor FontWarp
        {
            get
            {
                return this._fontWarp;
            }
            set
            {
                this._fontWarp = value;
            }
        }
        /// <summary>
        /// Amount of background noise to apply to the Captcha image.
        /// </summary>
        public CaptchaImage.BackgroundNoiseLevel BackgroundNoise
        {
            get
            {
                return this._backgroundNoise;
            }
            set
            {
                this._backgroundNoise = value;
            }
        }
        public CaptchaImage.LineNoiseLevel LineNoise
        {
            get
            {
                return this._lineNoise;
            }
            set
            {
                this._lineNoise = value;
            }
        }
        /// <summary>
        /// A string of valid characters to use in the Captcha text. 
        /// A random character will be selected from this string for each character.
        /// </summary>
        public string TextChars
        {
            get
            {
                return this._randomTextChars;
            }
            set
            {
                this._randomTextChars = value;
                this._randomText = this.GenerateRandomText();
            }
        }
        /// <summary>
        /// Number of characters to use in the Captcha text. 
        /// </summary>
        public int TextLength
        {
            get
            {
                return this._randomTextLength;
            }
            set
            {
                this._randomTextLength = value;
                this._randomText = this.GenerateRandomText();
            }
        }
        /// <summary>
        /// Returns the randomly generated Captcha text.
        /// </summary>
        public string Text
        {
            get
            {
                return this._randomText;
            }
        }
        /// <summary>
        /// Width of Captcha image to generate, in pixels 
        /// </summary>
        public int Width
        {
            get
            {
                return this._width;
            }
            set
            {
                if (value <= 60)
                {
                    throw new ArgumentOutOfRangeException("width", value, "width must be greater than 60.");
                }
                this._width = value;
            }
        }
        /// <summary>
        /// Height of Captcha image to generate, in pixels 
        /// </summary>
        public int Height
        {
            get
            {
                return this._height;
            }
            set
            {
                if (value <= 30)
                {
                    throw new ArgumentOutOfRangeException("height", value, "height must be greater than 30.");
                }
                this._height = value;
            }
        }
        /// <summary>
        /// A semicolon-delimited list of valid fonts to use when no font is provided.
        /// </summary>
        public string FontWhitelist
        {
            get
            {
                return this._fontWhitelist;
            }
            set
            {
                this._fontWhitelist = value;
            }
        }
        /// <summary>
        /// Background color for the captcha image
        /// </summary>
        public Color BackColor
        {
            get
            {
                return this._backColor;
            }
            set
            {
                this._backColor = value;
            }
        }
        /// <summary>
        /// Color of captcha text
        /// </summary>
        public Color FontColor
        {
            get
            {
                return this._fontColor;
            }
            set
            {
                this._fontColor = value;
            }
        }
        /// <summary>
        /// Color for dots in the background noise 
        /// </summary>
        public Color NoiseColor
        {
            get
            {
                return this._noiseColor;
            }
            set
            {
                this._noiseColor = value;
            }
        }
        /// <summary>
        /// Color for the background lines of the captcha image
        /// </summary>
        public Color LineColor
        {
            get
            {
                return this._lineColor;
            }
            set
            {
                this._lineColor = value;
            }
        }
        public CaptchaImage()
        {
            this._rand = new Random();
            this._fontWarp = CaptchaImage.FontWarpFactor.Low;
            this._backgroundNoise = CaptchaImage.BackgroundNoiseLevel.Low;
            this._lineNoise = CaptchaImage.LineNoiseLevel.None;
            this._width = 180;
            this._height = 50;
            this._randomTextLength = 5;
            this._randomTextChars = "ACDEFGHJKLNPQRTUVXYZ2346789";
            this._fontFamilyName = "";
            this._fontWhitelist = "arial;arial black;comic sans ms;courier new;estrangelo edessa;franklin gothic medium;georgia;lucida console;lucida sans unicode;mangal;microsoft sans serif;palatino linotype;sylfaen;tahoma;times new roman;trebuchet ms;verdana";
            this._randomText = this.GenerateRandomText();
            this._generatedAt = DateTime.Now;
            this._guid = Guid.NewGuid().ToString();
        }
        /// <summary>
        /// Forces a new Captcha image to be generated using current property value settings.
        /// </summary>
        public Bitmap RenderImage()
        {
            return this.GenerateImagePrivate();
        }
        /// <summary>
        /// Returns a random font family from the font whitelist
        /// </summary>
        private string RandomFontFamily()
        {
            string[] array = null;
            if (array == null)
            {
                array = this._fontWhitelist.Split(new char[]
                {
                    ';'
                });
            }
            return array[this._rand.Next(0, array.Length)];
        }
        /// <summary>
        /// generate random text for the CAPTCHA
        /// </summary>
        private string GenerateRandomText()
        {
            StringBuilder stringBuilder = new StringBuilder(this._randomTextLength);
            int length = this._randomTextChars.Length;
            for (int i = 0; i <= this._randomTextLength - 1; i++)
            {
                stringBuilder.Append(this._randomTextChars.Substring(this._rand.Next(length), 1));
            }
            return stringBuilder.ToString();
        }
        /// <summary>
        /// Returns a random point within the specified x and y ranges
        /// </summary>
        private PointF RandomPoint(int xmin, int xmax, int ymin, int ymax)
        {
            return new PointF((float)this._rand.Next(xmin, xmax), (float)this._rand.Next(ymin, ymax));
        }
        /// <summary>
        /// Returns a random point within the specified rectangle
        /// </summary>
        private PointF RandomPoint(Rectangle rect)
        {
            return this.RandomPoint(rect.Left, rect.Width, rect.Top, rect.Bottom);
        }
        /// <summary>
        /// Returns a GraphicsPath containing the specified string and font
        /// </summary>
        private GraphicsPath TextPath(string s, Font f, Rectangle r)
        {
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Near;
            stringFormat.LineAlignment = StringAlignment.Near;
            GraphicsPath graphicsPath = new GraphicsPath();
            graphicsPath.AddString(s, f.FontFamily, (int)f.Style, f.Size, r, stringFormat);
            return graphicsPath;
        }
        /// <summary>
        /// Returns the CAPTCHA font in an appropriate size 
        /// </summary>
        private Font GetFont()
        {
            float emSize = 0f;
            string text = this._fontFamilyName;
            if (text == "")
            {
                text = this.RandomFontFamily();
            }
            switch (this.FontWarp)
            {
                case CaptchaImage.FontWarpFactor.None:
                    emSize = (float)Convert.ToInt32((double)this._height * 0.7);
                    break;
                case CaptchaImage.FontWarpFactor.Low:
                    emSize = (float)Convert.ToInt32((double)this._height * 0.8);
                    break;
                case CaptchaImage.FontWarpFactor.Medium:
                    emSize = (float)Convert.ToInt32((double)this._height * 0.85);
                    break;
                case CaptchaImage.FontWarpFactor.High:
                    emSize = (float)Convert.ToInt32((double)this._height * 0.9);
                    break;
                case CaptchaImage.FontWarpFactor.Extreme:
                    emSize = (float)Convert.ToInt32((double)this._height * 0.95);
                    break;
            }
            return new Font(text, emSize, FontStyle.Bold);
        }
        /// <summary>
        /// Renders the CAPTCHA image
        /// </summary>
        private Bitmap GenerateImagePrivate()
        {
            Bitmap bitmap = new Bitmap(this._width, this._height, PixelFormat.Format32bppArgb);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle rect = new Rectangle(0, 0, this._width, this._height);
                Brush brush2;
                Brush brush = brush2 = new SolidBrush(this._backColor);
                try
                {
                    graphics.FillRectangle(brush, rect);
                }
                finally
                {
                    if (brush2 != null)
                    {
                        ((IDisposable)brush2).Dispose();
                    }
                }
                int num = 0;
                double num2 = (double)(this._width / this._randomTextLength);
                Brush brush3;
                brush = (brush3 = new SolidBrush(this._fontColor));
                try
                {
                    string randomText = this._randomText;
                    for (int i = 0; i < randomText.Length; i++)
                    {
                        char c = randomText[i];
                        Font font;
                        Font f = font = this.GetFont();
                        try
                        {
                            Rectangle rectangle = new Rectangle(Convert.ToInt32((double)num * num2), 0, Convert.ToInt32(num2), this._height);
                            using (GraphicsPath graphicsPath = this.TextPath(c.ToString(), f, rectangle))
                            {
                                this.WarpText(graphicsPath, rectangle);
                                graphics.FillPath(brush, graphicsPath);
                            }
                        }
                        finally
                        {
                            if (font != null)
                            {
                                ((IDisposable)font).Dispose();
                            }
                        }
                        num++;
                    }
                }
                finally
                {
                    if (brush3 != null)
                    {
                        ((IDisposable)brush3).Dispose();
                    }
                }
                this.AddNoise(graphics, rect);
                this.AddLine(graphics, rect);
            }
            return bitmap;
        }
        /// <summary>
        /// Warp the provided text GraphicsPath by a variable amount
        /// </summary>
        private void WarpText(GraphicsPath textPath, Rectangle rect)
        {
            float num = 1f;
            float num2 = 1f;
            switch (this._fontWarp)
            {
                case CaptchaImage.FontWarpFactor.None:
                    return;
                case CaptchaImage.FontWarpFactor.Low:
                    num = 6f;
                    num2 = 1f;
                    break;
                case CaptchaImage.FontWarpFactor.Medium:
                    num = 5f;
                    num2 = 1.3f;
                    break;
                case CaptchaImage.FontWarpFactor.High:
                    num = 4.5f;
                    num2 = 1.4f;
                    break;
                case CaptchaImage.FontWarpFactor.Extreme:
                    num = 4f;
                    num2 = 1.5f;
                    break;
            }
            RectangleF srcRect = new RectangleF(Convert.ToSingle(rect.Left), 0f, Convert.ToSingle(rect.Width), (float)rect.Height);
            int num3 = Convert.ToInt32((float)rect.Height / num);
            int num4 = Convert.ToInt32((float)rect.Width / num);
            int num5 = rect.Left - Convert.ToInt32((float)num4 * num2);
            int num6 = rect.Top - Convert.ToInt32((float)num3 * num2);
            int num7 = rect.Left + rect.Width + Convert.ToInt32((float)num4 * num2);
            int num8 = rect.Top + rect.Height + Convert.ToInt32((float)num3 * num2);
            if (num5 < 0)
            {
                num5 = 0;
            }
            if (num6 < 0)
            {
                num6 = 0;
            }
            if (num7 > this.Width)
            {
                num7 = this.Width;
            }
            if (num8 > this.Height)
            {
                num8 = this.Height;
            }
            PointF pointF = this.RandomPoint(num5, num5 + num4, num6, num6 + num3);
            PointF pointF2 = this.RandomPoint(num7 - num4, num7, num6, num6 + num3);
            PointF pointF3 = this.RandomPoint(num5, num5 + num4, num8 - num3, num8);
            PointF pointF4 = this.RandomPoint(num7 - num4, num7, num8 - num3, num8);
            PointF[] destPoints = new PointF[]
            {
                pointF,
                pointF2,
                pointF3,
                pointF4
            };
            Matrix matrix = new Matrix();
            matrix.Translate(0f, 0f);
            textPath.Warp(destPoints, srcRect, matrix, WarpMode.Perspective, 0f);
        }
        /// <summary>
        /// Add a variable level of graphic noise to the image
        /// </summary>
        private void AddNoise(Graphics graphics1, Rectangle rect)
        {
            int num = 0;
            int num2 = 0;
            switch (this._backgroundNoise)
            {
                case CaptchaImage.BackgroundNoiseLevel.None:
                    return;
                case CaptchaImage.BackgroundNoiseLevel.Low:
                    num = 30;
                    num2 = 40;
                    break;
                case CaptchaImage.BackgroundNoiseLevel.Medium:
                    num = 18;
                    num2 = 40;
                    break;
                case CaptchaImage.BackgroundNoiseLevel.High:
                    num = 16;
                    num2 = 39;
                    break;
                case CaptchaImage.BackgroundNoiseLevel.Extreme:
                    num = 12;
                    num2 = 38;
                    break;
            }
            using (SolidBrush solidBrush = new SolidBrush(this._noiseColor))
            {
                int maxValue = Convert.ToInt32(Math.Max(rect.Width, rect.Height) / num2);
                for (int i = 0; i <= Convert.ToInt32(rect.Width * rect.Height / num); i++)
                {
                    graphics1.FillEllipse(solidBrush, this._rand.Next(rect.Width), this._rand.Next(rect.Height), this._rand.Next(maxValue), this._rand.Next(maxValue));
                }
            }
        }
        /// <summary>
        /// Add variable level of curved lines to the image
        /// </summary>
        private void AddLine(Graphics graphics1, Rectangle rect)
        {
            int num = 0;
            float width = 1f;
            int num2 = 0;
            switch (this._lineNoise)
            {
                case CaptchaImage.LineNoiseLevel.None:
                    return;
                case CaptchaImage.LineNoiseLevel.Low:
                    num = 4;
                    width = Convert.ToSingle((double)this._height / 31.25);
                    num2 = 1;
                    break;
                case CaptchaImage.LineNoiseLevel.Medium:
                    num = 5;
                    width = Convert.ToSingle((double)this._height / 27.7777);
                    num2 = 1;
                    break;
                case CaptchaImage.LineNoiseLevel.High:
                    num = 3;
                    width = Convert.ToSingle(this._height / 25);
                    num2 = 2;
                    break;
                case CaptchaImage.LineNoiseLevel.Extreme:
                    num = 3;
                    width = Convert.ToSingle((double)this._height / 22.7272);
                    num2 = 3;
                    break;
            }
            PointF[] array = new PointF[num + 1];
            using (Pen pen = new Pen(this._lineColor, width))
            {
                for (int i = 1; i <= num2; i++)
                {
                    for (int j = 0; j <= num; j++)
                    {
                        array[j] = this.RandomPoint(rect);
                    }
                    graphics1.DrawCurve(pen, array, 1.75f);
                }
            }
        }
    }
}
