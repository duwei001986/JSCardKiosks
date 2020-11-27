using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Windows.Forms;
using System.Web.Services.Description;
using System.Reflection;
using System.CodeDom;
using System.Xml.Serialization;
using System.CodeDom.Compiler;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using WorkLog;
using System.Data.OleDb;
using System.Data;
using Emgu.CV;
using Emgu.CV.Structure;

namespace JSCardKiosks
{
    class PublicFuction
    {
        /// <summary>
        /// 无损压缩图片
        /// </summary>
        /// <param name="sFile">原图片地址</param>
        /// <param name="dFile">压缩后保存图片地址</param>
        /// <param name="flag">压缩质量（数字越小压缩率越高）1-100</param>
        /// <param name="size">压缩后图片的最大大小</param>
        /// <param name="sfsc">是否是第一次调用</param>
        /// <returns></returns>
        public static bool CompressImage(string sFile, string dFile, int flag = 90, int size = 300, bool sfsc = true)
        {
            //如果是第一次调用，原始图像的大小小于要压缩的大小，则直接复制文件，并且返回true
            FileInfo firstFileInfo = new FileInfo(sFile);
            if (sfsc == true && firstFileInfo.Length < size * 1024)
            {
                firstFileInfo.CopyTo(dFile);
                return true;
            }
            System.Drawing.Image iSource = System.Drawing.Image.FromFile(sFile);
            ImageFormat tFormat = iSource.RawFormat;
            int dHeight = iSource.Height / 2;
            int dWidth = iSource.Width / 2;
            int sW = 0, sH = 0;
            //按比例缩放
            Size tem_size = new Size(iSource.Width, iSource.Height);
            if (tem_size.Width > dHeight || tem_size.Width > dWidth)
            {
                if ((tem_size.Width * dHeight) > (tem_size.Width * dWidth))
                {
                    sW = dWidth;
                    sH = (dWidth * tem_size.Height) / tem_size.Width;
                }
                else
                {
                    sH = dHeight;
                    sW = (tem_size.Width * dHeight) / tem_size.Height;
                }
            }
            else
            {
                sW = tem_size.Width;
                sH = tem_size.Height;
            }

            Bitmap ob = new Bitmap(dWidth, dHeight);
            Graphics g = Graphics.FromImage(ob);

            g.Clear(Color.WhiteSmoke);
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            g.DrawImage(iSource, new Rectangle((dWidth - sW) / 2, (dHeight - sH) / 2, sW, sH), 0, 0, iSource.Width, iSource.Height, GraphicsUnit.Pixel);

            g.Dispose();

            //以下代码为保存图片时，设置压缩质量
            EncoderParameters ep = new EncoderParameters();
            long[] qy = new long[1];
            qy[0] = flag;//设置压缩的比例1-100
            EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
            ep.Param[0] = eParam;

            try
            {
                ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo jpegICIinfo = null;
                for (int x = 0; x < arrayICI.Length; x++)
                {
                    if (arrayICI[x].FormatDescription.Equals("JPEG"))
                    {
                        jpegICIinfo = arrayICI[x];
                        break;
                    }
                }
                if (jpegICIinfo != null)
                {
                    ob.Save(dFile, jpegICIinfo, ep);//dFile是压缩后的新路径
                    FileInfo fi = new FileInfo(dFile);
                    if (fi.Length > 1024 * size)
                    {
                        flag = flag - 10;
                        CompressImage(sFile, dFile, flag, size, false);
                    }
                }
                else
                {
                    ob.Save(dFile, tFormat);
                }
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                iSource.Dispose();
                ob.Dispose();
            }
        }
        #region 图片转为base64编码的字符串
        public static string ImgToBase64String(string Imagefilename)
        {
            try
            {
                Bitmap bmp = new Bitmap(Imagefilename);
                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                string strbase64 = Convert.ToBase64String(arr);
                arr = null;
                bmp.Dispose();
                return strbase64;

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion
        public static void SoundPlay(string fileName)
        {
            SoundPlayer player = new SoundPlayer();
            player.SoundLocation = Application.StartupPath + "\\" + fileName;
            player.Load();
            player.Play();
        }
        public static object GetDataByWebService(string url, string menthod, object[] args)
        {
            string strPath = Application.StartupPath;
            try
            {

                WebClient client = new WebClient();
                Stream stream = client.OpenRead(url);
                ServiceDescription description = ServiceDescription.Read(stream);
                string className = description.Services[0].Name;
                if (!File.Exists(strPath + "\\OutService.dll"))
                {
                    ServiceDescriptionImporter import = new ServiceDescriptionImporter();
                    import.ProtocolName = "soap";
                    import.Style = ServiceDescriptionImportStyle.Client;
                    import.CodeGenerationOptions = CodeGenerationOptions.GenerateProperties | CodeGenerationOptions.GenerateNewAsync;
                    import.AddServiceDescription(description, "", "");
                    CodeNamespace nmspace = new CodeNamespace(); //命名空间
                    nmspace.Name = "dweb";
                    CodeCompileUnit unit = new CodeCompileUnit();
                    unit.Namespaces.Add(nmspace);
                    ServiceDescriptionImportWarnings warning = import.Import(nmspace, unit);
                    CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
                    CompilerParameters parameter = new CompilerParameters();
                    parameter.GenerateExecutable = false;
                    parameter.OutputAssembly = "OutService.dll";//输出程序集的名称
                    parameter.ReferencedAssemblies.Add("System.dll");
                    parameter.ReferencedAssemblies.Add("System.XML.dll");
                    parameter.ReferencedAssemblies.Add("System.Web.Services.dll");
                    parameter.ReferencedAssemblies.Add("System.Data.dll");
                    CompilerResults result = provider.CompileAssemblyFromDom(parameter, unit);
                }

                Assembly asm = Assembly.LoadFrom("OutService.dll");//加载前面生成的程序集
                Type t = asm.GetType("dweb." + className);
                object o = Activator.CreateInstance(t);
                MethodInfo method = t.GetMethod(menthod);//服务端的方法名称
                object item = method.Invoke(o, args);
                return item;
            }

            catch (Exception ex)
            {
                return null;
            }

        }
        public static void SavePhoto(Image img, string path)
        {
            ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);
            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 100L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            img.Save(path, jgpEncoder, myEncoderParameters);
            //img.Save(path, ImageFormat.Jpeg);
        }
        public static void CutPicture(Bitmap bmpImage, string path, int x, int y, int width, int height)
        {
            System.Drawing.Rectangle cropArea = new System.Drawing.Rectangle(x, y, width, height);
            Bitmap bmpCrop = bmpImage.Clone(cropArea, bmpImage.PixelFormat);
            bmpCrop.SetResolution(350, 350);
            GC.Collect();
            try
            {
                Bitmap bSavePhoto = new Bitmap(441, 441);
                bSavePhoto.SetResolution(350, 350);
                Graphics g = Graphics.FromImage(bSavePhoto);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(bmpCrop, new Rectangle(0, 0, 441, 441), new Rectangle(0, 0, bmpCrop.Width, bmpCrop.Height), GraphicsUnit.Pixel);
                g.Dispose();
                ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);
                System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 85L);
                myEncoderParameters.Param[0] = myEncoderParameter;
                bSavePhoto.Save(path, jgpEncoder, myEncoderParameters);
                bmpCrop.Dispose();
            }
            catch
            {
                return;
            }
        }
        public static Image CutPicture(Bitmap bmpImage)
        {
            int x = bmpImage.Width * 200/ 1024;//
           // x = 0;
            int y = 0;
            int width = bmpImage.Width * 624 / 1024;
            int height = bmpImage.Height;
            System.Drawing.Rectangle cropArea = new System.Drawing.Rectangle(x, y, width, height);
            Bitmap bmpCrop = bmpImage.Clone(cropArea, bmpImage.PixelFormat);
            bmpCrop.SetResolution(350, 350);
            GC.Collect();
            try
            {
                Bitmap bSavePhoto = new Bitmap(358, 441);
                bSavePhoto.SetResolution(350, 350);
                Graphics g = Graphics.FromImage(bSavePhoto);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(bmpCrop, new Rectangle(0, 0, 358, 441), new Rectangle(0, 0, bmpCrop.Width, bmpCrop.Height), GraphicsUnit.Pixel);
                g.Dispose();
                return bSavePhoto;
            }
            catch
            {
                return null;
            }
        }
        public static ImageCodecInfo GetEncoder(ImageFormat format)
        {

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        /// <summary>
        /// 实现图像的磨皮和美白。
        /// <param name="Src">需要处理的源图像的数据结构。</param>
        /// <param name="Dest">需要处理的源图像的数据结构。</param>
        /// <param name="DenoiseLevel">磨皮的程度，有效范围[1,10]，数据越大，磨皮越明显。</param>
        /// <param name="WhiteMethod">美白的算法，0为Log曲线美白，1为色彩平衡磨皮。</param>
        /// <param name="WhiteLevel">美白的程度，有效范围[1,10]，数据越大，美白越明显。</param>
        /// <param name="TextureLevel">纹理的程度，有效范围[1,10]，数据越大，纹理越明显。</param>
        ///	<remarks>原图、目标图必须都是24位的。</remarks>

        [DllImport("SkinBeautification.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern int IM_NewMakeUp(IntPtr Src, IntPtr Dest, int Width, int Height, int Stride, int DenoiseLevel, int WhiteLevel, int TextureLevel);
        public static Bitmap GetGifImage(string strPicName)
        {
            DataTable dt = new DataTable();
            string strSql = "select imgName,gImg from imgList where imgName ='" + strPicName + "'";
            using (OleDbConnection conn = new OleDbConnection(Access_SQLHelper.strConn))
            {
                using (OleDbDataAdapter da = new OleDbDataAdapter(strSql, conn))
                {
                    try
                    {
                        conn.Open();
                        da.Fill(dt);
                    }
                    catch (Exception exErr)
                    {
                        WriteWorkLog.WriteLogs("日志", "异常", " 本地数据库异常(getGifList):" + exErr.ToString());
                        return null;
                    }
                }
            }
            byte[] imagebytes = (byte[])dt.Rows[0][1];
            MemoryStream ms = new MemoryStream(imagebytes);
            Bitmap bmpt = new Bitmap(ms);
            imagebytes = null;
            return bmpt;
        }

        public static object RunDllStaticFun(string dllPath, string className, string MethodName, object[] parts)
        {
            Assembly ass = Assembly.LoadFile(dllPath);
            Type type = ass.GetType(className);
            MethodInfo method = type.GetMethod(MethodName);
            return method.Invoke(null, parts);
        }
        public static int CutPhoto(string strSrcPath, string strDePath)
        {
            var face = new CascadeClassifier("haarcascade_frontalface_alt.xml");
            var img = new Image<Bgr, byte>(strSrcPath);//从文件加载图片
            var img2 = new Image<Gray, byte>(img.ToBitmap());
            //把图片从彩色转灰度
            CvInvoke.CvtColor(img, img2, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
            img.Dispose();
            //亮度增强
            CvInvoke.EqualizeHist(img2, img2);
            //在这一步就已经识别出来了,返回的是人脸所在的位置和大小
            var facesDetected = face.DetectMultiScale(img2, 1.02, 2, new Size(50, 50));
            if (facesDetected.Length == 0)
            {
                return -1;
            }
            int x = facesDetected[0].X;
            int y = facesDetected[0].Y;
            int cx = facesDetected[0].Width;
            int cy = facesDetected[0].Height;
            FileStream fs = new FileStream(strSrcPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader br = new BinaryReader(fs);
            MemoryStream ms = new MemoryStream(br.ReadBytes((int)fs.Length));
            fs.Close();
            Image image = Image.FromStream(ms);
            if (CutPicture2((Bitmap)image, strDePath, x, y, cx, cy) != 0)
            {
                return -2;
            }
            image.Dispose();
            img.Dispose();
            img2.Dispose();
            face.Dispose();
            return 0;
        }
        public static int CutPicture2(Bitmap bmpImage, string path, int x, int y, int widthFace, int heightFace)
        {

            int ScrCY = bmpImage.Height;
            int ScrCX = bmpImage.Width;
            int width = widthFace * 358 / 220;
            int height = heightFace * 441 / 220;
            if (height > ScrCY)
            {
                height = ScrCY;
                y = 0;
                width = height * 358 / 441;
                x = (ScrCX - width) / 2 + x - (ScrCX - widthFace) / 2;

            }
            else
            {
                y = y - widthFace * 95 / 220;
                if (y < 0)
                    y = 0;
                x = (ScrCX - width) / 2 + x - (ScrCX - widthFace) / 2; ;
            }
            Bitmap bmpCrop = null;
            try
            {
                System.Drawing.Rectangle cropArea = new System.Drawing.Rectangle(x, y, width, height);
                bmpCrop = bmpImage.Clone(cropArea, bmpImage.PixelFormat);
                bmpCrop.SetResolution(350, 350);
            }
            catch (Exception)
            {

                return -1;
            }

            GC.Collect();
            try
            {
                Bitmap bSavePhoto = new Bitmap(width, height);
                bSavePhoto.SetResolution(350, 350);
                Graphics g = Graphics.FromImage(bSavePhoto);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(bmpCrop, new Rectangle(0, 0, width, height), new Rectangle(0, 0, bmpCrop.Width, bmpCrop.Height), GraphicsUnit.Pixel);
                g.Dispose();
                ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);
                System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 95L);
                myEncoderParameters.Param[0] = myEncoderParameter;

                bSavePhoto.Save(path, jgpEncoder, myEncoderParameters);
                bmpCrop.Dispose();
            }
            catch
            {
                return -2;
            }
            return 0;
        }

        public static void Image_Resize(MemoryStream ms, string currentPath,int CX,int CY)  //处理文件的图片宽度并；生成新图片
        {
            Image img = Image.FromStream(ms);
            Bitmap newImg = new Bitmap(CX, CY, PixelFormat.Format24bppRgb);
            newImg.SetResolution(350, 350);
            Graphics g = Graphics.FromImage(newImg);
            g.DrawImage(img, 0, 0, CX, CY);
            ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);
            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 99L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            newImg.Save(currentPath, jgpEncoder, myEncoderParameters);
            g.Dispose();
            img.Dispose();
            newImg.Dispose();
        }
        public static void cutScrPhoto(Image img,string path)
        {
            Bitmap newbm = new Bitmap(img.Height*4/3, img.Height);
            newbm.SetResolution(350, 350);
            Graphics g = Graphics.FromImage(newbm);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.DrawImage(img, new Rectangle(-(img.Width/8), 0, img.Width, img.Height), new Rectangle(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
            g.Dispose();
            ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);
            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 95L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            newbm.Save(path, jgpEncoder, myEncoderParameters);
        }
        public static void cutScrPhoto(Image img, int x, int y, int nowWidth, int nowHeight)
        {
            Bitmap newbm = new Bitmap(nowWidth, nowHeight);
            newbm.SetResolution(350, 350);
            Graphics g = Graphics.FromImage(newbm);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.DrawImage(img, new Rectangle(x, y, img.Width, img.Height), new Rectangle(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
            g.Dispose();
            ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);
            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 95L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            newbm.Save("temp.jpg", jgpEncoder, myEncoderParameters);
        }
        public static void MakeSmallImg(System.IO.Stream fromFileStream, string fileSaveUrl, System.Double templateWidth, System.Double templateHeight)
        {
            //从文件取得图片对象，并使用流中嵌入的颜色管理信息
            System.Drawing.Image myImage = System.Drawing.Image.FromStream(fromFileStream, true);
            //缩略图宽、高
            System.Double newWidth = myImage.Width, newHeight = myImage.Height;
            //宽大于模版的横图
            if (myImage.Width > myImage.Height || myImage.Width == myImage.Height)
            {
                if (myImage.Width > templateWidth)
                {
                    //宽按模版，高按比例缩放
                    newWidth = templateWidth;
                    newHeight = myImage.Height * (newWidth / myImage.Width);
                }
            }
            //高大于模版的竖图
            else
            {
                if (myImage.Height > templateHeight)
                {
                    //高按模版，宽按比例缩放
                    newHeight = templateHeight;
                    newWidth = myImage.Width * (newHeight / myImage.Height);
                }
            }
            //取得图片大小
            System.Drawing.Size mySize = new Size((int)newWidth, (int)newHeight);
            //新建一个bmp图片
            System.Drawing.Image bitmap = new System.Drawing.Bitmap(mySize.Width, mySize.Height);
            //新建一个画板
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);
            //设置高质量插值法
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //清空一下画布
            g.Clear(Color.White);
            //在指定位置画图
            g.DrawImage(myImage, new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
            new System.Drawing.Rectangle(0, 0, myImage.Width, myImage.Height),
            System.Drawing.GraphicsUnit.Pixel);
            ///文字水印
            //System.Drawing.Graphics G=System.Drawing.Graphics.FromImage(bitmap);
            //System.Drawing.Font f=new Font("宋体",10);
            //System.Drawing.Brush b=new SolidBrush(Color.Black);
            //G.DrawString("myohmine",f,b,10,10);
            //G.Dispose();
            ///图片水印
            //System.Drawing.Image copyImage = System.Drawing.Image.FromFile(System.Web.HttpContext.Current.Server.MapPath("pic/1.gif"));
            //Graphics a = Graphics.FromImage(bitmap);
            //a.DrawImage(copyImage, new Rectangle(bitmap.Width-copyImage.Width,bitmap.Height-copyImage.Height,copyImage.Width, copyImage.Height),0,0, copyImage.Width, copyImage.Height, GraphicsUnit.Pixel);
            //copyImage.Dispose();
            //a.Dispose();
            //copyImage.Dispose();
            //保存缩略图
           
            bitmap.Save(fileSaveUrl, System.Drawing.Imaging.ImageFormat.Jpeg);
            g.Dispose();
            myImage.Dispose();
            bitmap.Dispose();
        }
    }
}
