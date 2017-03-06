using System;
using System.Collections;
using System.Drawing;
using System.IO;

namespace DKP.Web.Framework.Security.Captcha.Gif
{
    public class GifDecoder
    {
        protected int[] act;
        protected int bgColor;
        protected int bgIndex;
        protected Bitmap bitmap;
        protected byte[] block = new byte[0x100];
        protected int blockSize = 0;
        protected int delay = 0;
        protected int dispose = 0;
        protected int frameCount;
        protected ArrayList frames;
        protected int[] gct;
        protected bool gctFlag;
        protected int gctSize;
        protected int height;
        protected int ih;
        protected Image image;
        protected Stream inStream;
        protected bool interlace;
        protected int iw;
        protected int ix;
        protected int iy;
        protected int lastBgColor;
        protected int lastDispose = 0;
        protected Image lastImage;
        protected Rectangle lastRect;
        protected int[] lct;
        protected bool lctFlag;
        protected int lctSize;
        protected int loopCount = 1;
        protected static readonly int MaxStackSize = 0x1000;
        protected int pixelAspect;
        protected byte[] pixels;
        protected byte[] pixelStack;
        protected short[] prefix;
        protected int status;
        public static readonly int STATUS_FORMAT_ERROR = 1;
        public static readonly int STATUS_OK = 0;
        public static readonly int STATUS_OPEN_ERROR = 2;
        protected byte[] suffix;
        protected int transIndex;
        protected bool transparency = false;
        protected int width;

        protected void DecodeImageData()
        {
            int num11;
            int num12;
            int num13;
            int num14;
            int num15;
            int num16;
            int num = -1;
            int num2 = this.iw * this.ih;
            if ((this.pixels == null) || (this.pixels.Length < num2))
            {
                this.pixels = new byte[num2];
            }
            if (this.prefix == null)
            {
                this.prefix = new short[MaxStackSize];
            }
            if (this.suffix == null)
            {
                this.suffix = new byte[MaxStackSize];
            }
            if (this.pixelStack == null)
            {
                this.pixelStack = new byte[MaxStackSize + 1];
            }
            int num3 = this.Read();
            int num4 = ((int) 1) << num3;
            int num5 = num4 + 1;
            int index = num4 + 2;
            int num7 = num;
            int num8 = num3 + 1;
            int num9 = (((int) 1) << num8) - 1;
            int num10 = 0;
            while (num10 < num4)
            {
                this.prefix[num10] = 0;
                this.suffix[num10] = (byte) num10;
                num10++;
            }
            int num17 = num16 = num15 = num14 = num13 = num12 = num11 = 0;
            int num18 = 0;
            while (num18 < num2)
            {
                if (num13 == 0)
                {
                    if (num16 < num8)
                    {
                        if (num15 == 0)
                        {
                            num15 = this.ReadBlock();
                            if (num15 <= 0)
                            {
                                break;
                            }
                            num11 = 0;
                        }
                        num17 += (this.block[num11] & 0xff) << num16;
                        num16 += 8;
                        num11++;
                        num15--;
                        continue;
                    }
                    num10 = num17 & num9;
                    num17 = num17 >> num8;
                    num16 -= num8;
                    if ((num10 > index) || (num10 == num5))
                    {
                        break;
                    }
                    if (num10 == num4)
                    {
                        num8 = num3 + 1;
                        num9 = (((int) 1) << num8) - 1;
                        index = num4 + 2;
                        num7 = num;
                        continue;
                    }
                    if (num7 == num)
                    {
                        this.pixelStack[num13++] = this.suffix[num10];
                        num7 = num10;
                        num14 = num10;
                        continue;
                    }
                    int num19 = num10;
                    if (num10 == index)
                    {
                        this.pixelStack[num13++] = (byte) num14;
                        num10 = num7;
                    }
                    while (num10 > num4)
                    {
                        this.pixelStack[num13++] = this.suffix[num10];
                        num10 = this.prefix[num10];
                    }
                    num14 = this.suffix[num10] & 0xff;
                    if (index >= MaxStackSize)
                    {
                        break;
                    }
                    this.pixelStack[num13++] = (byte) num14;
                    this.prefix[index] = (short) num7;
                    this.suffix[index] = (byte) num14;
                    index++;
                    if (((index & num9) == 0) && (index < MaxStackSize))
                    {
                        num8++;
                        num9 += index;
                    }
                    num7 = num19;
                }
                num13--;
                this.pixels[num12++] = this.pixelStack[num13];
                num18++;
            }
            for (num18 = num12; num18 < num2; num18++)
            {
                this.pixels[num18] = 0;
            }
        }

        protected bool Error()
        {
            return (this.status != STATUS_OK);
        }

        public int GetDelay(int n)
        {
            this.delay = -1;
            if ((n >= 0) && (n < this.frameCount))
            {
                this.delay = ((GifFrame) this.frames[n]).delay;
            }
            return this.delay;
        }

        public Image GetFrame(int n)
        {
            Image image = null;
            if ((n >= 0) && (n < this.frameCount))
            {
                image = ((GifFrame) this.frames[n]).image;
            }
            return image;
        }

        public int GetFrameCount()
        {
            return this.frameCount;
        }

        public Size GetFrameSize()
        {
            return new Size(this.width, this.height);
        }

        public Image GetImage()
        {
            return this.GetFrame(0);
        }

        public int GetLoopCount()
        {
            return this.loopCount;
        }

        private int[] GetPixels(Bitmap bitmap)
        {
            int[] numArray = new int[(3 * this.image.Width) * this.image.Height];
            int index = 0;
            for (int i = 0; i < this.image.Height; i++)
            {
                for (int j = 0; j < this.image.Width; j++)
                {
                    Color pixel = bitmap.GetPixel(j, i);
                    numArray[index] = pixel.R;
                    index++;
                    numArray[index] = pixel.G;
                    index++;
                    numArray[index] = pixel.B;
                    index++;
                }
            }
            return numArray;
        }

        protected void Init()
        {
            this.status = STATUS_OK;
            this.frameCount = 0;
            this.frames = new ArrayList();
            this.gct = null;
            this.lct = null;
        }

        protected int Read()
        {
            int num = 0;
            try
            {
                num = this.inStream.ReadByte();
            }
            catch (IOException)
            {
                this.status = STATUS_FORMAT_ERROR;
            }
            return num;
        }

        public int Read(Stream inStream)
        {
            this.Init();
            if (inStream != null)
            {
                this.inStream = inStream;
                this.ReadHeader();
                if (!this.Error())
                {
                    this.ReadContents();
                    if (this.frameCount < 0)
                    {
                        this.status = STATUS_FORMAT_ERROR;
                    }
                }
                inStream.Close();
            }
            else
            {
                this.status = STATUS_OPEN_ERROR;
            }
            return this.status;
        }

        public int Read(string name)
        {
            this.status = STATUS_OK;
            try
            {
                name = name.Trim().ToLower();
                this.status = this.Read(new FileInfo(name).OpenRead());
            }
            catch (IOException)
            {
                this.status = STATUS_OPEN_ERROR;
            }
            return this.status;
        }

        protected int ReadBlock()
        {
            this.blockSize = this.Read();
            int offset = 0;
            if (this.blockSize <= 0)
            {
                return offset;
            }
            try
            {
                int num2 = 0;
                while (offset < this.blockSize)
                {
                    num2 = this.inStream.Read(this.block, offset, this.blockSize - offset);
                    if (num2 == -1)
                    {
                        goto Label_007A;
                    }
                    offset += num2;
                }
            }
            catch (IOException)
            {
            }
        Label_007A:
            if (offset < this.blockSize)
            {
                this.status = STATUS_FORMAT_ERROR;
            }
            return offset;
        }

        protected int[] ReadColorTable(int ncolors)
        {
            int num = 3 * ncolors;
            int[] numArray = null;
            byte[] buffer = new byte[num];
            int num2 = 0;
            try
            {
                num2 = this.inStream.Read(buffer, 0, buffer.Length);
            }
            catch (IOException)
            {
            }
            if (num2 < num)
            {
                this.status = STATUS_FORMAT_ERROR;
                return numArray;
            }
            numArray = new int[0x100];
            int num3 = 0;
            int num4 = 0;
            while (num3 < ncolors)
            {
                int num5 = buffer[num4++] & 0xff;
                int num6 = buffer[num4++] & 0xff;
                int num7 = buffer[num4++] & 0xff;
                numArray[num3++] = (((unchecked((int) 0xff000000L)) | (num5 << 0x10)) | (num6 << 8)) | num7;
            }
            return numArray;
        }

        protected void ReadContents()
        {
            bool flag = false;
            while (!flag && !this.Error())
            {
                switch (this.Read())
                {
                    case 0x2c:
                        this.ReadImage();
                        break;

                    case 0x3b:
                        flag = true;
                        break;

                    case 0:
                        break;

                    case 0x21:
                        switch (this.Read())
                        {
                            case 0xf9:
                                this.ReadGraphicControlExt();
                                break;

                            case 0xff:
                            {
                                this.ReadBlock();
                                string str = "";
                                for (int i = 0; i < 11; i++)
                                {
                                    str = str + ((char) this.block[i]);
                                }
                                if (str.Equals("NETSCAPE2.0"))
                                {
                                    this.ReadNetscapeExt();
                                }
                                else
                                {
                                    this.Skip();
                                }
                                break;
                            }
                        }
                        this.Skip();
                        break;

                    default:
                        this.status = STATUS_FORMAT_ERROR;
                        break;
                }
            }
        }

        protected void ReadGraphicControlExt()
        {
            this.Read();
            int num = this.Read();
            this.dispose = (num & 0x1c) >> 2;
            if (this.dispose == 0)
            {
                this.dispose = 1;
            }
            this.transparency = (num & 1) != 0;
            this.delay = this.ReadShort() * 10;
            this.transIndex = this.Read();
            this.Read();
        }

        protected void ReadHeader()
        {
            string str = "";
            for (int i = 0; i < 6; i++)
            {
                str = str + ((char) this.Read());
            }
            if (!str.StartsWith("GIF"))
            {
                this.status = STATUS_FORMAT_ERROR;
            }
            else
            {
                this.ReadLSD();
                if (!(!this.gctFlag || this.Error()))
                {
                    this.gct = this.ReadColorTable(this.gctSize);
                    this.bgColor = this.gct[this.bgIndex];
                }
            }
        }

        protected void ReadImage()
        {
            this.ix = this.ReadShort();
            this.iy = this.ReadShort();
            this.iw = this.ReadShort();
            this.ih = this.ReadShort();
            int num = this.Read();
            this.lctFlag = (num & 0x80) != 0;
            this.interlace = (num & 0x40) != 0;
            this.lctSize = ((int) 2) << (num & 7);
            if (this.lctFlag)
            {
                this.lct = this.ReadColorTable(this.lctSize);
                this.act = this.lct;
            }
            else
            {
                this.act = this.gct;
                if (this.bgIndex == this.transIndex)
                {
                    this.bgColor = 0;
                }
            }
            int num2 = 0;
            if (this.transparency)
            {
                num2 = this.act[this.transIndex];
                this.act[this.transIndex] = 0;
            }
            if (this.act == null)
            {
                this.status = STATUS_FORMAT_ERROR;
            }
            if (!this.Error())
            {
                this.DecodeImageData();
                this.Skip();
                if (!this.Error())
                {
                    this.frameCount++;
                    this.bitmap = new Bitmap(this.width, this.height);
                    this.image = this.bitmap;
                    this.SetPixels();
                    this.frames.Add(new GifFrame(this.bitmap, this.delay));
                    if (this.transparency)
                    {
                        this.act[this.transIndex] = num2;
                    }
                    this.ResetFrame();
                }
            }
        }

        protected void ReadLSD()
        {
            this.width = this.ReadShort();
            this.height = this.ReadShort();
            int num = this.Read();
            this.gctFlag = (num & 0x80) != 0;
            this.gctSize = ((int) 2) << (num & 7);
            this.bgIndex = this.Read();
            this.pixelAspect = this.Read();
        }

        protected void ReadNetscapeExt()
        {
            do
            {
                this.ReadBlock();
                if (this.block[0] == 1)
                {
                    int num = this.block[1] & 0xff;
                    int num2 = this.block[2] & 0xff;
                    this.loopCount = (num2 << 8) | num;
                }
            }
            while ((this.blockSize > 0) && !this.Error());
        }

        protected int ReadShort()
        {
            return (this.Read() | (this.Read() << 8));
        }

        protected void ResetFrame()
        {
            this.lastDispose = this.dispose;
            this.lastRect = new Rectangle(this.ix, this.iy, this.iw, this.ih);
            this.lastImage = this.image;
            this.lastBgColor = this.bgColor;
            this.lct = null;
        }

        protected void SetPixels()
        {
            int[] pixels = this.GetPixels(this.bitmap);
            if (this.lastDispose > 0)
            {
                if (this.lastDispose == 3)
                {
                    int num = this.frameCount - 2;
                    if (num > 0)
                    {
                        this.lastImage = this.GetFrame(num - 1);
                    }
                    else
                    {
                        this.lastImage = null;
                    }
                }
                if (this.lastImage != null)
                {
                    Array.Copy(this.GetPixels(new Bitmap(this.lastImage)), 0, pixels, 0, this.width * this.height);
                    if (this.lastDispose == 2)
                    {
                        Graphics graphics = Graphics.FromImage(this.image);
                        Color empty = Color.Empty;
                        if (this.transparency)
                        {
                            empty = Color.FromArgb(0, 0, 0, 0);
                        }
                        else
                        {
                            empty = Color.FromArgb(this.lastBgColor);
                        }
                        Brush brush = new SolidBrush(empty);
                        graphics.FillRectangle(brush, this.lastRect);
                        brush.Dispose();
                        graphics.Dispose();
                    }
                }
            }
            int num2 = 1;
            int num3 = 8;
            int num4 = 0;
            for (int i = 0; i < this.ih; i++)
            {
                int num6 = i;
                if (this.interlace)
                {
                    if (num4 >= this.ih)
                    {
                        num2++;
                        switch (num2)
                        {
                            case 2:
                                num4 = 4;
                                break;

                            case 3:
                                num4 = 2;
                                num3 = 4;
                                break;

                            case 4:
                                num4 = 1;
                                num3 = 2;
                                break;
                        }
                    }
                    num6 = num4;
                    num4 += num3;
                }
                num6 += this.iy;
                if (num6 < this.height)
                {
                    int num8 = num6 * this.width;
                    int index = num8 + this.ix;
                    int num10 = index + this.iw;
                    if ((num8 + this.width) < num10)
                    {
                        num10 = num8 + this.width;
                    }
                    int num11 = i * this.iw;
                    while (index < num10)
                    {
                        int num12 = this.pixels[num11++] & 0xff;
                        int num13 = this.act[num12];
                        if (num13 != 0)
                        {
                            pixels[index] = num13;
                        }
                        index++;
                    }
                }
            }
            this.SetPixels(pixels);
        }

        private void SetPixels(int[] pixels)
        {
            int num = 0;
            for (int i = 0; i < this.image.Height; i++)
            {
                for (int j = 0; j < this.image.Width; j++)
                {
                    Color color = Color.FromArgb(pixels[num++]);
                    this.bitmap.SetPixel(j, i, color);
                }
            }
        }

        protected void Skip()
        {
            do
            {
                this.ReadBlock();
            }
            while ((this.blockSize > 0) && !this.Error());
        }

        public class GifFrame
        {
            public int delay;
            public Image image;

            public GifFrame(Image im, int del)
            {
                this.image = im;
                this.delay = del;
            }
        }
    }
}

