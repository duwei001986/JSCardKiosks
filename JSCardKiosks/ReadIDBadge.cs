using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using WorkLog;

namespace CZIDCardReader
{
    enum _IDS_RET
    {
        _IDS_RET_ERR = -50,             //一般错误
        _IDS_RET_ERR_JAM,               //卡住
        _IDS_RET_ERR_MEDIA,             //卡介质不存在
        _IDS_RET_ERR_SIDE,              //不支持的扫描面向类型
        _IDS_RET_ERR_MODE,              //不支持的颜色模式
        _IDS_RET_ERR_TYPE,              //不支持的卡类型
        _IDS_RET_ERR_DPI,               //不支持的分辨率
        _IDS_RET_ERR_DELAY,             //延时时间超出范围
        _IDS_RET_ERR_RF,                //RF读取失败
        _IDS_RET_ERR_UNSUPPORTED,       //不支持的功能
        _IDS_RET_OK = 0,                //状态正常    
        _IDS_RET_BUSY                   //设备正忙
    }

    public struct DEV_INFO
    {
        public uint ulVid;
        public uint ulPid;
        public uint iFileDesc;

    }

    public struct _IDS_DEV_DESC
    {
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
        public DEV_INFO dev;
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 60)]
        public Byte[] acDevName;

    }
    public enum EN_IF_CHL_ID : int
    {
        EN_IF_CHL_ID_USB = 0x01,     //USB
        EN_IF_CHL_ID_BT,                //蓝牙
        EN_IF_CHL_ID_UART,              //串口
        EN_IF_CHL_ID_WIFI,              //WIFI
        EN_IF_CHL_ID_NET                //网络
    }
    // [StructLayout(LayoutKind.Sequential)]
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct ST_DEV_NAME_INFO
    {
        //[MarshalAs(UnmanagedType.U4)]
        public EN_IF_CHL_ID enChl;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 88)]
        public string acName;

    }

    public enum _IDS_DEV_STATUS_TYPE
    {
        _IDS_DEV_STATUS_TYPE_DS = 0x00000001,          //数字信号状态检测标志
    }

    public struct _IDS_STATUS_RESULT
    {
        public _IDS_DEV_DS_STATUS _Sensor;
    }

    public struct _IDS_DEV_DS_STATUS
    {
        public int ulCOVER;
        public int ulSensor1;
        public int ulSensor2;
        public int ulSensor3;
        public int ulSensor4;
        public int ulSlpSta;
        public int ulRsv;
    }

;
    public enum _IDS_MOVE_POS //: int
    {
        _IDS_MOVE_POS_INSERT = 0,               //移到用户插卡方向
        _IDS_MOVE_POS_SCAN,                     //移到扫描准备位置
        _IDS_MOVE_POS_CALI,                     //移到校正准备位置
        _IDS_MOVE_POS_RECLAIM,                  //卡片回收方向
        _IDS_MOVE_POS_EJECT_HALF,               //卡片RF位置
        _IDS_MOVE_POS_PERLOAD                   //预加载卡片
    }
    ;
    public enum _IDS_CARD_TYPE
    {
        _IDS_CARD_TYPE_IMAGE = 1, //普通卡，仅扫描图像
        _IDS_CARD_TYPE_ID_CN = 2, //中国居民身份证(含外国人居留证)[固定扫描区域&自动面向识别&自动裁切]
        _IDS_CARD_TYPE_PASS_CN = 3  //港澳台通行证[固定扫描区域&自动面向识别&自动裁切]
    }
    ;


    public struct ST_CARD_INFO_IC_GEN
    {
        public uint ulDataLen;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1000)]
        public Byte[] aucData;
    }
 ;
    //扫描颜色
    public enum _IDS_SCAN_MODE : int
    {
        _IDS_SCAN_MODE_COLOR = 0,      //彩色模式
        _IDS_SCAN_MODE_GRAY_R,         //灰度(R)
        _IDS_SCAN_MODE_GRAY_G,         //灰度(G)
        _IDS_SCAN_MODE_GRAY_B,         //灰度(B)
        _IDS_SCAN_MODE_GRAY_IR,        //灰度(IR)
    }
    ;

    //扫描面向
    public enum _IDS_SCAN_SIDE : int
    {
        _IDS_SCAN_SIDE_FRONT = 0x01, //上部扫描标示
        _IDS_SCAN_SIDE_BACK = 0x02, //下部扫描标示
        _IDS_SCAN_SIDE_DUP = 0x03  //双面扫描标示
    }
    ;

    //扫描分辨率
    public enum _IDS_SCAN_DPI : int
    {
        _IDS_SCAN_DPI_150 = 150,
        _IDS_SCAN_DPI_300 = 300,
    }
        ;

    //图像调整参数
    public struct CardInfo
    {
        public string name;
        public string Gender;
        public string Nation;
        public string Birth;
        public string Address;
        public string IDNumber;
        public string DateBegin;
        public string DateEnd;
    }
    public struct _IDS_IMAGE_PARAM
    {
        //[MarshalAs(UnmanagedType.I4)]
        public int lBrightness;//明亮度参数 0~100
        //[MarshalAs(UnmanagedType.I4)]
        public int lContrast;//对比度参数 0~100
                             // [MarshalAs(UnmanagedType.I4)]
        public int lGamma;                //影像Gamma调整曲线参数 0 ~ 150
    }
;
    // [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct _IDS_SCAN_CONF
    {
        public _IDS_CARD_TYPE _Type;
        public _IDS_SCAN_MODE _Mode;
        public _IDS_SCAN_SIDE _Side;
        public _IDS_SCAN_DPI _Dpi;
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public _IDS_IMAGE_PARAM _Front;
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public _IDS_IMAGE_PARAM _Back;
    }
;
    [StructLayout(LayoutKind.Sequential)]
    public struct _IDS_SCAN_RESULT
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string cFileNameFront;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string cFileNameBack;
        public byte ucCardDir;
        public byte ucCardType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2)]
        public string aucRsv;
    }
 ;
    public enum EN_MOVE_CARD_MODE : int
    {
        EN_MOVE_CARD_MODE_NONE = 0,     //移卡到扫描位置、校正位置以及半退卡时设置
        EN_MOVE_CARD_MODE_DIRECT,       //扫描移卡、回收移卡时设置
        EN_MOVE_CARD_MODE_HOLD,         //扫描移卡、回收移卡时设置
    }
    ;
    public enum EN_MOVE_CARD_SPEED : int
    {
        EN_MOVE_CARD_SPEED_DEF = 0,
        EN_MOVE_CARD_SPEED_SLOW,
    }
    ;

    public enum EN_IDS_SHUTTER_TYPE : int
    {
        EN_IDS_SHUTTER_FRONT = 0,    //前端卡口
        EN_IDS_SHUTTER_BACK = 1     //后端卡口
    }
    ;

    public enum EN_IDS_SHUTTER_CAP : int
    {
        EN_IDS_SHUTTER_ENABLE = 0,    //前端卡口
        EN_IDS_SHUTTER_DISABLE = 1     //后端卡口
    }
    public class ReadIDBadge
    {
        [DllImport("IDSIF.dll", EntryPoint = "EnumDev", SetLastError = true,
           CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int EnumDev(ref uint puiTotal, IntPtr pstDevList);

        [DllImport("IDSIF.dll", EntryPoint = "GetStatus", SetLastError = true,
            CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetStatus(_IDS_DEV_STATUS_TYPE _Type, out _IDS_STATUS_RESULT pResult);

        [DllImport("IDSIF.dll", EntryPoint = "Init", SetLastError = true,
            CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Init(Byte[] pcFileSavePath, IntPtr pstDevDesc);

        [DllImport("IDSIF.dll", EntryPoint = "Uninit", SetLastError = true,
          CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Uninit();
        [DllImport("IDSIF.dll", EntryPoint = "Move", SetLastError = true,
           CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Move(_IDS_MOVE_POS _Pos, uint ulTimeout);

        [DllImport("IDSIF.dll", EntryPoint = "ReadRF", SetLastError = true,
            CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        //public static extern int ReadRF(_IDS_CARD_TYPE  _CardType,IntPtr pRFResult, uint  ulTimeout);
        public static extern int ReadRF(_IDS_CARD_TYPE _CardType, byte[] pRFResult, uint ulTimeout);

        [DllImport("IDSIF.dll", EntryPoint = "Scan", SetLastError = true,
           CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Scan(_IDS_SCAN_CONF _ScanConf, IntPtr pResult);

        [DllImport("IDSIF.dll", EntryPoint = "MoveEx", SetLastError = true,
           CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MoveEx(_IDS_MOVE_POS _Pos, EN_MOVE_CARD_MODE enMode,
            EN_MOVE_CARD_SPEED enSpeed, uint ulTimeout);

        [DllImport("IDSIF.dll", EntryPoint = "CtrlShutter", SetLastError = true,
           CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern int CtrlShutter(EN_IDS_SHUTTER_TYPE _shutterType, EN_IDS_SHUTTER_CAP _shutterCap);

        public static int IDReaderOpen(string strPath)
        {
            uint uiDevTotal = 0;
            int ret = -1;
            IntPtr DevList;
            DevList = Marshal.AllocHGlobal(100);
            for (int i = 0; i < 100; i++)
                Marshal.WriteByte(DevList, i, 0x00);
            ret = EnumDev(ref uiDevTotal, DevList);
            Marshal.FreeHGlobal(DevList);
            if (ret != 0)
            {
                WriteWorkLog.WriteLogs("日志", "Err", "未搜索到身份证阅读器设备");
                return ret;
            }
            _IDS_DEV_DESC stdevdesc = new _IDS_DEV_DESC();
            stdevdesc.dev.ulVid = 0x05e3;
            stdevdesc.dev.ulPid = 0x0102;
            stdevdesc.dev.iFileDesc = 0;
            IntPtr stdevdes;
            stdevdes = Marshal.AllocHGlobal(100);
            for (int i = 0; i < 100; i++)
                Marshal.WriteByte(stdevdes, i, 0x00);

            Marshal.StructureToPtr(stdevdesc, stdevdes, false);
            if (!Directory.Exists(strPath))
            {
                Directory.CreateDirectory(strPath);
            }
            byte[] ch = new byte[20];
            ch = System.Text.Encoding.Default.GetBytes(strPath);
            ret = -1;
            ret = Init(ch, stdevdes);
            Marshal.FreeHGlobal(stdevdes);
            if (ret == 0)
            {
                return 0;
            }
            else
            {
                WriteWorkLog.WriteLogs("日志", "Err", "身份证阅读器打开失败");
                return ret;
            }
        }
        public static int CardInDisable()
        {
            int iret = CtrlShutter(EN_IDS_SHUTTER_TYPE.EN_IDS_SHUTTER_FRONT, EN_IDS_SHUTTER_CAP.EN_IDS_SHUTTER_DISABLE);
            iret = CtrlShutter(EN_IDS_SHUTTER_TYPE.EN_IDS_SHUTTER_BACK, EN_IDS_SHUTTER_CAP.EN_IDS_SHUTTER_DISABLE);
            return iret;
        }
        public static int CardInEnable()
        {
            int iret = CtrlShutter(EN_IDS_SHUTTER_TYPE.EN_IDS_SHUTTER_FRONT, EN_IDS_SHUTTER_CAP.EN_IDS_SHUTTER_ENABLE);
            iret = CtrlShutter(EN_IDS_SHUTTER_TYPE.EN_IDS_SHUTTER_BACK, EN_IDS_SHUTTER_CAP.EN_IDS_SHUTTER_ENABLE);
            return iret;
        }
        public static int IDCardRead(string strPath, out CardInfo IDInfo)
        {
            IDInfo = new CardInfo();
            byte[] _RFResult = new byte[3000];
            _IDS_MOVE_POS move = _IDS_MOVE_POS._IDS_MOVE_POS_EJECT_HALF;
            int ret = -1;
            ret = Move(move, 1000);
            _IDS_CARD_TYPE type;
            type = _IDS_CARD_TYPE._IDS_CARD_TYPE_ID_CN;
            IntPtr RFResult = IntPtr.Zero;
            int size = 3500;
            RFResult = Marshal.AllocHGlobal(size); // 为指针分配空间
            int lRFRet = ReadRF(type, _RFResult, 1000);
            byte[] name = new byte[30];
            System.Array.Copy(_RFResult, 0, name, 0, 30);
            string name1 = System.Text.Encoding.Default.GetString(name);
            name1 = name1.Replace("\0", "");
            byte[] ucGender = new byte[30];
            System.Array.Copy(_RFResult, 60, ucGender, 0, 4);
            string ucGender1 = System.Text.Encoding.Default.GetString(ucGender);
            ucGender1 = ucGender1.Replace("\0", "");
            byte[] ucNation = new byte[30];
            System.Array.Copy(_RFResult, 70, ucNation, 0, 4);
            string ucNation1 = System.Text.Encoding.Default.GetString(ucNation);
            ucNation1 = ucNation1.Replace("\0", "");
            byte[] ucBirth = new byte[30];
            System.Array.Copy(_RFResult, 170, ucBirth, 0, 10);
            string ucBirth1 = System.Text.Encoding.Default.GetString(ucBirth);
            ucBirth1 = ucBirth1.Replace("\0", "");
            byte[] ucAddress = new byte[80];
            System.Array.Copy(_RFResult, 185, ucAddress, 0, 70);
            string ucAddress1 = System.Text.Encoding.Default.GetString(ucAddress);
            ucAddress1 = ucAddress1.Replace("\0", "");
            byte[] ucIDNumber = new byte[30];
            System.Array.Copy(_RFResult, 287, ucIDNumber, 0, 18);
            string ucIDNumber1 = System.Text.Encoding.Default.GetString(ucIDNumber);
            ucIDNumber1 = ucIDNumber1.Replace("\0", "");
            byte[] ucIssued = new byte[32];
            System.Array.Copy(_RFResult, 344, ucIssued, 0, 30);
            string ucIssued1 = System.Text.Encoding.Default.GetString(ucIssued);
            ucIssued1 = ucIssued1.Replace("\0", "");
            byte[] ucDateBegin = new byte[30];
            System.Array.Copy(_RFResult, 403, ucDateBegin, 0, 8);
            string ucDateBegin1 = System.Text.Encoding.Default.GetString(ucDateBegin);
            ucDateBegin1 = ucDateBegin1.Replace("\0", "");
            byte[] ucDateEnd = new byte[30];
            System.Array.Copy(_RFResult, 419, ucDateEnd, 0, 8);
            string ucDateEnd1 = System.Text.Encoding.Default.GetString(ucDateEnd);
            ucDateEnd1 = ucDateEnd1.Replace("\0", "");
            if (ucGender1 == "2")
            {
                ucGender1 = "女";
            }
            if (ucGender1 == "1")
            {
                ucGender1 = "男";
            }
            IDInfo.name = name1;
            IDInfo.Gender = ucGender1;
            IDInfo.Nation = ucNation1;
            IDInfo.Birth = ucBirth1;
            IDInfo.Address = ucAddress1;
            IDInfo.IDNumber = ucIDNumber1;
            IDInfo.DateBegin = ucDateBegin1;
            IDInfo.DateEnd = ucDateEnd1;
            Bitmap btm = (Bitmap)Image.FromFile(strPath + "\\head.bmp");
            MemoryStream ms = new MemoryStream();
            btm.Save(ms, btm.RawFormat);
            var eps = new EncoderParameters(1);
            var ep = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 85L);
            eps.Param[0] = ep;
            var jpsEncodeer = GetEncoder(ImageFormat.Jpeg);
            string strSavePath = AppDomain.CurrentDomain.BaseDirectory + "IDPhoto";
            if (!Directory.Exists(strSavePath))
            {
                Directory.CreateDirectory(strSavePath);
            }
            if (!File.Exists(strSavePath + "\\" + ucIDNumber1 + "_r.jpg"))
                btm.Save(strSavePath + "\\" + ucIDNumber1 + "_r.jpg", jpsEncodeer, eps);
            //释放资源
            btm.Dispose();
            ep.Dispose();
            eps.Dispose();
            return 0;
        }
        public static int IDCardScan()
        {
            MoveEx(_IDS_MOVE_POS._IDS_MOVE_POS_SCAN, EN_MOVE_CARD_MODE.EN_MOVE_CARD_MODE_DIRECT, EN_MOVE_CARD_SPEED.EN_MOVE_CARD_SPEED_DEF, 10000);
            _IDS_SCAN_CONF _ScanConf = new _IDS_SCAN_CONF();
            _IDS_SCAN_RESULT _ScanResult = new _IDS_SCAN_RESULT();
            _ScanConf._Dpi = _IDS_SCAN_DPI._IDS_SCAN_DPI_300;// m_objDlgSetting.GetScanConf()._Dpi;
            _ScanConf._Mode = _IDS_SCAN_MODE._IDS_SCAN_MODE_COLOR;//m_objDlgSetting.GetScanConf()._Mode;
            _ScanConf._Side = _IDS_SCAN_SIDE._IDS_SCAN_SIDE_DUP;//m_objDlgSetting.GetScanConf()._Side;
            _ScanConf._Type = _IDS_CARD_TYPE._IDS_CARD_TYPE_ID_CN;// m_objDlgSetting.GetScanConf()._Type;
            _ScanConf._Front = new _IDS_IMAGE_PARAM();
            _ScanConf._Back = new _IDS_IMAGE_PARAM();
            _ScanConf._Front.lBrightness = Convert.ToInt32(0);
            _ScanConf._Front.lContrast = Convert.ToInt32(0);
            _ScanConf._Front.lGamma = Convert.ToInt32(100);
            _ScanConf._Back.lBrightness = Convert.ToInt32(0);
            _ScanConf._Back.lContrast = Convert.ToInt32(0);
            _ScanConf._Back.lGamma = Convert.ToInt32(100);
            IntPtr scan1;
            int size = 3500;
            scan1 = Marshal.AllocHGlobal(size); // 为指针分配空间
            for (int i = 0; i < size; i++)
                Marshal.WriteByte(scan1, i, 0x00);
            int ret = Scan(_ScanConf, scan1);
            _ScanResult = (_IDS_SCAN_RESULT)Marshal.PtrToStructure(scan1, typeof(_IDS_SCAN_RESULT));
            Marshal.FreeHGlobal(scan1);
            return ret;
        }
        public static int CardEject()
        {
            return MoveEx(_IDS_MOVE_POS._IDS_MOVE_POS_INSERT, EN_MOVE_CARD_MODE.EN_MOVE_CARD_MODE_HOLD, EN_MOVE_CARD_SPEED.EN_MOVE_CARD_SPEED_DEF, 10000);
        }
        public static int IDReaderClose()
        {
            return Uninit();
        }
        public static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                    return codec;
            }
            return null;
        }
    }
}
