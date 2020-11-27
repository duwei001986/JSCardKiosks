
using AForge.Controls;
using AForge.Video.DirectShow;
using MediaPlayerHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using WorkLog;

namespace JSCardKiosks
{
    public partial class TakePhoto : Form
    {
        [DllImport("shell32.dll")]
        public static extern int ShellExecute(IntPtr hwnd, StringBuilder lpszOp, StringBuilder lpszFile, StringBuilder lpszParams, StringBuilder lpszDir, int FsShowCmd);

        public const int STEP_1_TAKEPHOTOINFO = 1;
        public const int STEP_2_TAKEPHOTO = 2;
        public const int STEP_3_CHOOSEPHOTO = 3;
        public const int STEP_4_SUBMITPHOTO = 4;
        bool bReadyClick = true;
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoDevice;
        int iTime = 10;
        bool bHasReady = false;
        int iWaitNum = 3;
        MainForm mainForm;
        int iStep = 0;
        IdInfo idInfo = new IdInfo();
        int iPhotoNo = 0;
        Image oldImg1 = null;
        Image oldImg2 = null;
        Image oldImg3 = null;
        Image lastImg1 = null;
        Image lastImg2 = null;
        Image lastImg3 = null;
        int iChoosePhoto = 2;
        public TakePhoto()
        {
            InitializeComponent();
        }
        public TakePhoto(IdInfo idInfo)
        {
            this.idInfo = idInfo;
            InitializeComponent();
        }
        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        CreateParams cp = base.CreateParams;
        //        cp.ExStyle |= 0x02000000;
        //        return cp;
        //    }
        //}
        public static void SetDouble(Control cc)
        {

            cc.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance |
                         System.Reflection.BindingFlags.NonPublic).SetValue(cc, true, null);

        }


        /// <summary>
        /// 拍照按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void labRetTime_Click(object sender, EventArgs e)
        {
            if (bReadyClick)
            {
                iTime = 6;
                bHasReady = true;
            }
            bReadyClick = false;
        }
        public ImageCodecInfo GetEncoder(ImageFormat format)
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
        private void vispShoot_Paint(object sender, PaintEventArgs e)
        {
            using (Image image = Properties.Resources.outline2)
            {
                Rectangle r = new Rectangle(0, 0, vispShoot.Width + 1, vispShoot.Height + 1);
                e.Graphics.DrawImage(image, r);

            }
            GC.Collect();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }
        public void savePhoto(string photoPath)
        {
            Bitmap img = vispShoot.GetCurrentVideoFrame();
            ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);
            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 95L);
            myEncoderParameters.Param[0] = myEncoderParameter;

            img.Save(photoPath, jgpEncoder, myEncoderParameters);
            // PublicFuction.CutPicture(img, photoPath, img.Width * 200 / 1024, 0, img.Width * 624 / 1024, img.Height);
            if (vispShoot.VideoSource != null)
                vispShoot.Stop();
        }
        private void TakePhoto_Load(object sender, EventArgs e)
        {
            palChoosePhoto.Location = new Point(0, 0);
            paldDterminePhoto.Location = new Point(0, 0);
            palInfo.Location = new Point(0, 0);
            mainForm = (MainForm)this.Owner;
            //timer1.Enabled = false;
            //vispShoot.Enabled = false;
            //return;
            iStep = STEP_1_TAKEPHOTOINFO;

            PublicFuction.SoundPlay("拍照前，请您仔细阅读以下注意事项！.wav");
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videoDevices.Count != 0)
            {
                for (int i = 0; i < videoDevices.Count; i++)
                {
                    string cameraName = videoDevices[i].Name;
                    if (videoDevices.Count == 1)
                    {
                        videoDevice = new VideoCaptureDevice(videoDevices[0].MonikerString);
                        break;
                    }
                    if (cameraName == "YC-FI3105C")
                    {
                        videoDevice = new VideoCaptureDevice(videoDevices[i].MonikerString);
                        break;
                    }

                }
            }
            else
            {
                MessageBox.Show("没有找到摄像头", "ABC");//
                return;
            }
            string strTemp = DealAppConfig.GetAppSettingsValue("摄像头分辨率");
            int iDpi = Convert.ToInt32(strTemp);
            videoDevice.VideoResolution = videoDevice.VideoCapabilities[Convert.ToInt32(strTemp)];
            showVideo();
        }
        private int showVideo()
        {
            if (videoDevice != null)
            {
                vispShoot.VideoSource = videoDevice;
                vispShoot.Start();
            }
            return 0;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            iTime--;
            if (palInfo.Visible)
            {
                iStep = STEP_1_TAKEPHOTOINFO;
            }
            if (!palInfo.Visible && !paldDterminePhoto.Visible && !palChoosePhoto.Visible)
            {
                iStep = STEP_2_TAKEPHOTO;
            }
            if (!palInfo.Visible && !paldDterminePhoto.Visible && palChoosePhoto.Visible)
            {
                iStep = STEP_3_CHOOSEPHOTO;
            }
            if (!palInfo.Visible && paldDterminePhoto.Visible && !palChoosePhoto.Visible)
            {
                iStep = STEP_4_SUBMITPHOTO;
            }
            switch (iStep)
            {
                case STEP_1_TAKEPHOTOINFO:
                    labInfoTime1.Text = iTime.ToString().PadLeft(2, '0');
                    if (iTime <= 0)
                    {
                        palInfo.Visible = false;
                        iTime = 60;
                        labRetTime2.Text = iTime.ToString().PadLeft(2, '0');
                        PublicFuction.SoundPlay("请按确认键3秒钟后开始拍照.wav");
                    }
                    break;
                case STEP_2_TAKEPHOTO:
                    labRetTime2.Text = iTime.ToString().PadLeft(2, '0');
                    if (bHasReady)
                    {
                        Bitmap img = vispShoot.GetCurrentVideoFrame();
                        //Thread.Sleep(300);
                        //Image imgPhoto = PublicFuction.CutPicture(img);
                        //imgPhoto = img;
                        if (iTime <= 5 && iTime >= 3)
                        {
                            labRetTime2.Text = (iTime - 2).ToString().PadLeft(2, '0');
                            PublicFuction.SoundPlay((iTime - 2).ToString() + ".wav");
                        }
                        if (iTime == 5)
                        {
                            lastImg1 = img.Clone() as System.Drawing.Image; oldImg1 = img.Clone() as System.Drawing.Image; photo1.Image = PublicFuction.CutPicture((Bitmap)oldImg1);//Image.Clone() as System.Drawing.Image;
                        }

                        if (iTime == 4)
                        {
                            lastImg2 = img.Clone() as System.Drawing.Image; oldImg2 = img.Clone() as System.Drawing.Image; photo2.Image = PublicFuction.CutPicture((Bitmap)oldImg2);
                        }
                        if (iTime == 3)
                        {
                            lastImg3 = img.Clone() as System.Drawing.Image; oldImg3 = img.Clone() as System.Drawing.Image; photo3.Image = PublicFuction.CutPicture((Bitmap)oldImg3);
                            vispShoot.SignalToStop();
                            vispShoot.WaitForStop();
                            bHasReady = false;
                            vispShoot.BackgroundImage = img;
                        }
                        img = null;
                    }
                    else
                    {
                        if (iTime == 1 && photo1.Image != null)
                        {
                            iTime = 30;
                            labsubmitPhoto3.Text = iTime.ToString().PadLeft(2, '0');
                            palChoosePhoto.Visible = true;
                            bReadyClick = true;
                            vispShoot.Visible = false;
                            iChoosePhoto = 2;
                            SetChoose(iChoosePhoto);
                        }
                        if (iTime <= 0 && vispShoot.BackgroundImage == null)
                        {
                            this.Close();
                        }
                    }
                    break;
                case STEP_3_CHOOSEPHOTO:
                    labsubmitPhoto3.Text = iTime.ToString().PadLeft(2, '0');
                    if (iTime <= 0)
                    {
                        this.Close();
                    }
                    break;
                case STEP_4_SUBMITPHOTO:
                    labSubmit4.Text = iTime.ToString().PadLeft(2, '0');
                    if (iTime <= 0)
                    {
                        this.Close();
                    }
                    break;
                default:
                    break;
            }
        }
        private void labSure_Click(object sender, EventArgs e)
        {
            //我已知晓确认
            palInfo.Visible = false;
            iTime = 60;
            labRetTime2.Text = iTime.ToString().PadLeft(2, '0');
            PublicFuction.SoundPlay("请按确认键3秒钟后开始拍照.wav");
        }
        private void photo_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.PictureBox pictureBox = (System.Windows.Forms.PictureBox)sender;
            iPhotoNo = Convert.ToInt32(pictureBox.Name.Substring(pictureBox.Name.Length - 1));
            SetChoose(iPhotoNo);
        }
        private void SetChoose(int i)
        {
            switch (i)
            {
                case 1:
                    iChoosePhoto = 1;
                    btnChoose2.Visible = false;
                    btnChoose3.Visible = false;
                    btnChoose1.Visible = true;
                    break;
                case 2:
                    iChoosePhoto = 2;
                    btnChoose1.Visible = false;
                    btnChoose3.Visible = false;
                    btnChoose2.Visible = true;
                    break;
                case 3:
                    iChoosePhoto = 3;
                    btnChoose1.Visible = false;
                    btnChoose2.Visible = false;
                    btnChoose3.Visible = true;
                    break;
                default:
                    break;
            }
        }

        private void labRePhoto_Click(object sender, EventArgs e)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "temp.jpg";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            photo1.Image = null;
            iTime = 60;
            labRetTime2.Text = iTime.ToString().PadLeft(2, '0');
            palChoosePhoto.Visible = false;
            PublicFuction.SoundPlay("请按确认键3秒钟后开始拍照.wav");
            vispShoot.Visible = true;
            vispShoot.Start();
            iStep = STEP_2_TAKEPHOTO;

        }

        private void labBeauty_Click(object sender, EventArgs e)
        {
            Image img = null;
            if (btnChoose1.Visible)
            {
                img = lastImg1.Clone() as System.Drawing.Image; ;
                BitmapData BmpDataS = ((Bitmap)img).LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                PublicFuction.IM_NewMakeUp(BmpDataS.Scan0, BmpDataS.Scan0, img.Width, img.Height, BmpDataS.Stride, 2, 1, 2);
                ((Bitmap)img).UnlockBits(BmpDataS);
                lastImg1 = img;
                photo1.Image = PublicFuction.CutPicture((Bitmap)img);
                img.Dispose();
            }
            if (btnChoose2.Visible)
            {
                img = lastImg2.Clone() as System.Drawing.Image; ;
                BitmapData BmpDataS = ((Bitmap)img).LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                PublicFuction.IM_NewMakeUp(BmpDataS.Scan0, BmpDataS.Scan0, img.Width, img.Height, BmpDataS.Stride, 2, 1, 2);
                ((Bitmap)img).UnlockBits(BmpDataS);
                lastImg2 = img;
                photo2.Image = PublicFuction.CutPicture((Bitmap)img);
                img.Dispose();
            }
            if (btnChoose3.Visible)
            {
                img = lastImg3.Clone() as System.Drawing.Image; ;
                BitmapData BmpDataS = ((Bitmap)img).LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                PublicFuction.IM_NewMakeUp(BmpDataS.Scan0, BmpDataS.Scan0, img.Width, img.Height, BmpDataS.Stride, 2, 1, 2);
                ((Bitmap)img).UnlockBits(BmpDataS);
                lastImg3 = img;
                photo3.Image = PublicFuction.CutPicture((Bitmap)img);
                img.Dispose();
            }
        }

        private void labCancelBeauty_Click(object sender, EventArgs e)
        {
            if (btnChoose1.Visible)
            {
                lastImg1 = oldImg1.Clone() as System.Drawing.Image; ;
                photo1.Image = oldImg1;// PublicFuction.CutPicture((Bitmap)oldImg1);
            }
            if (btnChoose2.Visible)
            {
                lastImg2 = oldImg2.Clone() as System.Drawing.Image; ;
                photo2.Image = PublicFuction.CutPicture((Bitmap)oldImg2);
            }
            if (btnChoose3.Visible)
            {
                lastImg3 = oldImg3.Clone() as System.Drawing.Image; ;
                photo3.Image = PublicFuction.CutPicture((Bitmap)oldImg3);
            }
        }

        private void labsubmitPhoto_Click(object sender, EventArgs e)
        {
            labSubmit4.Enabled = true;
            labWaitTxt.Visible = true;
            string path = AppDomain.CurrentDomain.BaseDirectory + "temp.jpg";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            if (iChoosePhoto == 1)
            {
                // PublicFuction.cutScrPhoto(lastImg1, -lastImg1.Width / 2+ oldImg1.Height / 2, 0, oldImg1.Height, oldImg1.Height);
                //PublicFuction.CutPicture((Bitmap)oldImg1, path, oldImg1.Width/2-oldImg1.Height/2, 0, oldImg1.Height , oldImg1.Height);
                PublicFuction.SavePhoto(oldImg1, path);
            }
            if (iChoosePhoto == 2)
            {
                //  PublicFuction.cutScrPhoto(lastImg2, -lastImg2.Width / 2 + lastImg2.Height / 2, 0, lastImg2.Height  , lastImg2.Height);
                PublicFuction.SavePhoto(oldImg2, path);
            }
            if (iChoosePhoto == 3)
            {
                // PublicFuction.cutScrPhoto(lastImg3, -oldImg3.Width/2+oldImg3.Height/2, 0, oldImg3.Height , oldImg3.Height);
                PublicFuction.SavePhoto(oldImg3, path);
            }
            iTime = 30;
            labSubmit4.Text = iTime.ToString().PadLeft(2, '0');
            Thread.Sleep(200);
            //FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            //BinaryReader br = new BinaryReader(fs);
            //MemoryStream ms = new MemoryStream(br.ReadBytes((int)fs.Length));
            //fs.Close();
            string strDealPhotoPath = DealAppConfig.GetAppSettingsValue("照片处理路径") + "\\";
            if (File.Exists(strDealPhotoPath + "1211_org.jpg"))
            {
                File.Delete(strDealPhotoPath + "1211_org.jpg");
            }
            Mp3Player player = new Mp3Player();
            player.FileName = "照片正在处理中，请稍后.mp3";
            player.play();
            //FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            //BinaryReader br = new BinaryReader(fs);
            //MemoryStream ms = new MemoryStream(br.ReadBytes((int)fs.Length));
            //Image img = Image.FromStream(ms);
            //string strTempPath = AppDomain.CurrentDomain.BaseDirectory + "temp2.jpg";
            //  PublicFuction.cutScrPhoto(img, strTempPath);
            int iret = ShellExecute(IntPtr.Zero, new StringBuilder("Open"), new StringBuilder("base64_person.exe"), new StringBuilder(@"--model_dir .\model.h5 --file_path " + path + " --bg_color 255 255 255 --mykey RXFG#@*UM --thred " + DealAppConfig.GetAppSettingsValue("抠图参数")), new StringBuilder(DealAppConfig.GetAppSettingsValue("照片处理路径")), 0);//.\temp.jpg
            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(200);

                if (File.Exists(strDealPhotoPath + "1211_org.jpg"))
                {
                    Thread.Sleep(500);
                    break;
                }
            }
            WriteWorkLog.WriteLogs("日志", "信息", "调用baseperson返回:" + iret.ToString());
            try
            {
                iret = PublicFuction.CutPhoto(strDealPhotoPath + "1211_org.jpg", AppDomain.CurrentDomain.BaseDirectory + idInfo.num + "_up.jpg");
            }
            catch (Exception)
            {

                iret = -2;
            }

            Thread.Sleep(150);

            if (iret != 0)
            {
                picBlankPhoto.Image = null;
                player = new Mp3Player();
                player.FileName = "照片处理失败请重新拍照.mp3";
                player.play();
                labSubmit4.Enabled = false;
            }
            else
            {
                FileStream fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + idInfo.num + "_up.jpg", FileMode.Open, FileAccess.Read, FileShare.Read);
                BinaryReader br = new BinaryReader(fs);
                MemoryStream ms = new MemoryStream(br.ReadBytes((int)fs.Length));
                picBlankPhoto.Image = Image.FromStream(ms);
                fs.Close();

            }

            labWaitTxt.Visible = false;
            palChoosePhoto.Visible = false;
            paldDterminePhoto.Visible = true;
        }

        private void labRTakePhoto_Click(object sender, EventArgs e)
        {
            paldDterminePhoto.Visible = false;
            photo1.Image = null;
            iTime = 60;
            labRetTime2.Text = iTime.ToString().PadLeft(2, '0');
            palChoosePhoto.Visible = false;
            PublicFuction.SoundPlay("请按确认键3秒钟后开始拍照.wav");
            vispShoot.Visible = true;
            vispShoot.Start();
            iStep = STEP_2_TAKEPHOTO;
        }
        private void TakePhoto_FormClosing(object sender, FormClosingEventArgs e)
        {
            vispShoot.Visible = false;
            timer1.Stop();
            mainForm.SetbtnRetmainVisable(false);
            mainForm.SetpalMidVisable(false);
            mainForm.SetHeadText("社会保障卡发卡终端");
            if (vispShoot.VideoSource != null)
            {
                vispShoot.Stop();
                vispShoot.VideoSource = null;
            }
            timer1.Dispose();
            this.Dispose();
        }

        private void labSubmit4_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            mainForm.SetHeadText("输入手机号码");
            mainForm.SetbtnRetmainVisable(true);
            object[] objPara = new object[1];
            objPara[0] = idInfo;
            mainForm.ShowChildForm("JSCardKiosks.InputPhoneNur", 1, objPara);
            timer1.Dispose();
            this.Dispose();

        }
    }
}
