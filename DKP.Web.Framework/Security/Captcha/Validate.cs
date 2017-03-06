using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using DKP.Web.Framework.Security.Captcha.Gif;

namespace DKP.Web.Framework.Security.Captcha
{
    public class Validate
    {
        public string AllChar = "1,2,3,4,5,6,7,8,9,0,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,W,X,Y,Z";
        public Color DrawColor = Color.BlueViolet;
        public bool FontTextRenderingHint = false;
        public int ImageHeight = 0x17;
        private byte TrueValidateCodeCount = 4;
        protected string ValidateCode = "";
        public string ValidateCodeFont = "Arial";
        public float ValidateCodeSize = 13f;

        private void CreateImageBmp(out Bitmap ImageFrame)
        {
            char[] chArray = this.ValidateCode.ToCharArray(0, this.ValidateCodeCount);
            int width = (int)(((this.TrueValidateCodeCount * this.ValidateCodeSize) * 1.3) + 4.0);
            ImageFrame = new Bitmap(width, this.ImageHeight);
            Graphics graphics = Graphics.FromImage(ImageFrame);
            graphics.Clear(Color.White);
            Font font = new Font(this.ValidateCodeFont, this.ValidateCodeSize, FontStyle.Bold);
            Brush brush = new SolidBrush(this.DrawColor);
            int maxValue = (int)Math.Max((float)((this.ImageHeight - this.ValidateCodeSize) - 3f), (float)2f);
            Random random = new Random();
            for (int i = 0; i < this.TrueValidateCodeCount; i++)
            {
                int[] numArray = new int[] { (((int)(i * this.ValidateCodeSize)) + random.Next(1)) + 3, random.Next(maxValue) };
                Point point = new Point(numArray[0], numArray[1]);
                if (this.FontTextRenderingHint)
                {
                    graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;
                }
                else
                {
                    graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                }
                graphics.DrawString(chArray[i].ToString(), font, brush, (PointF)point);
            }
            graphics.Dispose();
        }

        private byte[] CreateImageGif()
        {
            AnimatedGifEncoder encoder = new AnimatedGifEncoder();
            MemoryStream stream = new MemoryStream();
            encoder.Start();
            encoder.SetDelay(150);
            encoder.SetRepeat(0);
            for (int i = 0; i < 10; i++)
            {
                Bitmap bitmap;
                this.CreateImageBmp(out bitmap);
                this.DisposeImageBmp(ref bitmap);
                bitmap.Save(stream, ImageFormat.Png);
                encoder.AddFrame(Image.FromStream(stream));
                stream = new MemoryStream();
            }
            encoder.OutPut(ref stream);
            //HttpContext.Current.Response.ClearContent();
            //HttpContext.Current.Response.ContentType = "image/Gif";
            //HttpContext.Current.Response.BinaryWrite(stream.ToArray());
            //stream.Close();
            //stream.Dispose();
            return stream.GetBuffer();
        }

        private void CreateValidate()
        {
            this.ValidateCode = "";
            string[] strArray = this.AllChar.Split(new char[] { ',' });
            int index = -1;
            Random random = new Random();
            for (int i = 0; i < this.ValidateCodeCount; i++)
            {
                if (index != -1)
                {
                    random = new Random((i * index) * ((int)DateTime.Now.Ticks));
                }
                int num3 = random.Next(0x23);
                if (index == num3)
                {
                    this.CreateValidate();
                }
                index = num3;
                this.ValidateCode = this.ValidateCode + strArray[index];
            }
            if (this.ValidateCode.Length > this.TrueValidateCodeCount)
            {
                this.ValidateCode = this.ValidateCode.Remove(this.TrueValidateCodeCount);
            }
        }

        private void DisposeImageBmp(ref Bitmap ImageFrame)
        {
            Graphics graphics = Graphics.FromImage(ImageFrame);
            Pen pen = new Pen(this.DrawColor, 1f);
            Random random = new Random();
            Point[] pointArray = new Point[2];
            for (int i = 0; i < 15; i++)
            {
                pointArray[0] = new Point(random.Next(ImageFrame.Width), random.Next(ImageFrame.Height));
                pointArray[1] = new Point(random.Next(ImageFrame.Width), random.Next(ImageFrame.Height));
                graphics.DrawLine(pen, pointArray[0], pointArray[1]);
            }
            graphics.Dispose();
        }

        public byte[] OutPutValidate(string validateCodeSession)
        {
            this.CreateValidate();
            byte[] bytes = this.CreateImageGif();
            HttpContext.Current.Session[validateCodeSession] = this.ValidateCode;
            return bytes;
        }

        public byte ValidateCodeCount
        {
            get
            {
                return this.TrueValidateCodeCount;
            }
            set
            {
                if (value > 4)
                {
                    this.TrueValidateCodeCount = value;
                }
            }
        }
    }
}
