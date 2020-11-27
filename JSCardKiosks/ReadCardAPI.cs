using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace IDCardReader
{
    public static class ReadCardAPI
    {
		//////////////////////////////////////////////////////////////////////////
		//				SAM端口函数
		//
		//////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 查看串口当前波特率(该函数只用于SAM采用RS232串口的情形，如果采用USB接口则不支持该API)
		/// </summary>
		/// <param name="iPort">[in] 整数，表示端口号。此处端口号必须为1-16，表示串口</param>
		/// <param name="puiBaudRate">[out] 无符号整数指针，指向普通串口当前波特率, 默认情况下为115200</param>
		/// <returns>0成功，1失败/不合法，5无法获得该SAM的波特率，该SAM串口不可用</returns>
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_GetCOMBaud")]
        public static extern int Syn_GetCOMBaud(int iPort, ref int puiBaudRate);
		/// <summary>
		/// 设置SAM的串口的波特率(该函数只用于SAM采用RS232串口的情形，如果采用USB接口则不支持该API)，
		/// 设置成功后，在该SAM和主机注册表中都记录设置后的波特率，保证在SAM重新启动和该套API被重新调用时采用设置后的波特率。
		/// 该函数调用成功后，需要延时5毫秒，然后才能继续与SAM通信
		/// </summary>
		/// <param name="iPort">[in] 整数，表示端口号。此处端口号必须为1-16，表示串口。</param>
		/// <param name="uiCurrBaud">
		/// [in] 无符号整数，调用该API前已设置的业务终端与SAM通信的波特率(SAM出厂时默认,业务终端与SAM通信的波特率为115200).
		/// 业务终端以该波特率与SAM通信,发出设置SAM新波特率的命令.。
		/// uiCurrBaud只能为下列数值之一：115200，57600，38400，19200，9600.如果uiCurrBaud数值不是这些值之一，函数返回0X21;
		/// 如果已设置的波特率与uiCurrBaud不一致, 则函数返回0X02,表示不能设置,调用API不成功。
		/// </param>
		/// <param name="uiSetBaud">
		/// [in] 无符号整数，将要设置的SAM与业务终端通信波特率。
		/// uiSetBaud只能取下列值之一:：115200，57600，38400，19200，9600，
		/// 如果输入uiSetBaud参数不是这些数值之一,，函数返回0X21,设置不成功,保持原来的波特率不变。
		/// </param>
		/// <returns>
		/// 0成功，1端口打开失败/端口号不合法，2超时，设置不成功，33 uiCurrBaud 、uiSetBaud输入参数数值错误
		/// </returns>
        [DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_SetCOMBaud")]
        public static extern int Syn_SetCOMBaud(int iPort, int uiCurrBaud, int uiSetBaud);
		/// <summary>
		/// 打开端口
		/// </summary>
		/// <param name="iPort">端口号。1-16（十进制）为串口，1001-1016（十进制）为USB口，9999为USBHID</param>
		/// <returns>0成功，1打开端口失败/端口号不合法</returns>
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_OpenPort")]
        public static extern int Syn_OpenPort(int iPort);
		/// <summary>
		/// 关闭端口
		/// </summary>
		/// <param name="iPort">[in]整数，表示端口号</param>
		/// <returns>0成功，1端口不合法</returns>
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_ClosePort")]
        public static extern int Syn_ClosePort(int iPort);

        //////////////////////////////////////////////////////////////////////////
        //				SAM类函数
        //
        //////////////////////////////////////////////////////////////////////////
        [DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_SetMaxRFByte")]
        public static extern int Syn_SetMaxRFByte(int iPort,byte ucByte, int bIfOpen);
		/// <summary>
		/// 对SAM复位
		/// </summary>
		/// <param name="iPort">
		/// [in] 整数，表示端口号。
		/// 根据SAM使用的接口不同(分为普通串口SAM和USB口SAM)，
		/// 分别使用不同的端口号（目前串口和USB都只支持16个，即串口0001-0016和USB1001-1016）
		/// </param>
		/// <param name="iIfOpen">
		/// [in] 整数，0表示不在该函数内部打开和关闭串口，
		/// 此时确保之前调用了Syn_OpenPort来打开端口，并且在不需要与端口通信时，调用Syn_ClosePort关闭端口；
		/// 非0表示在API函数内部包含了打开端口和关闭端口函数，之前不需要调用Syn_OpenPort，也不用再调用Syn_ClosePort
		/// </param>
		/// <returns>0成功，其他失败（具体参见返回码表）</returns>
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_ResetSAM")]
        public static extern int Syn_ResetSAM(int iPort, int iIfOpen);
		/// <summary>
		/// 对SAM进行状态检测
		/// </summary>
		/// <param name="iPort">[in] 整数，表示端口号。参见Syn_ResetSAM</param>
		/// <param name="iIfOpen">[in] 整数，参见Syn_ResetSAM</param>
		/// <returns>0SAM正常,96自检失败，不能接收命令，其他，命令失败</returns>
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_GetSAMStatus")]
        public static extern int Syn_GetSAMStatus(int iPort, int iIfOpen);
		/// <summary>
		/// 读取SAM的编号
		/// </summary>
		/// <param name="iPort">[in] 整数，表示端口号。参见Syn_ResetSAM</param>
		/// <param name="pucSAMID">[out] 无符号字符串指针，指向读到的SAM编号， 16字节</param>
		/// <param name="iIfOpen"></param>
		/// <returns>0成功，其他，失败</returns>
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_GetSAMID")]
        public static extern int Syn_GetSAMID(int iPort, byte[] pucSAMID, int iIfOpen);
        [DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_GetSAMIDToStr")]
        public static extern int Syn_GetSAMIDToStr(int iPort, ref char[] pucSAMID, int iIfOpen);

		//////////////////////////////////////////////////////////////////////////
		//				身份证卡类函数
		//
		//////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 开始寻卡
		/// </summary>
		/// <param name="iPort">[in] 整数，表示端口号。参见Syn_ResetSAM</param>
		/// <param name="pucIIN">[out] 无符号字符指针，指向读到的IIN</param>
		/// <param name="iIfOpen">[in] 整数，参见Syn_ResetSAM</param>
		/// <returns>0成功，128失败</returns>
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_StartFindIDCard")]
        public static extern int Syn_StartFindIDCard(int iPort, ref byte[] pucIIN, int iIfOpen);
		/// <summary>
		/// 选卡
		/// </summary>
		/// <param name="iPort">[in] 整数，表示端口号。参见Syn_ResetSAM</param>
		/// <param name="pucIIN">[out] 无符号字符指针，指向读到的SN</param>
		/// <param name="iIfOpen">[in] 整数，参见Syn_ResetSAM</param>
		/// <returns>0选卡成功，129选卡失败</returns>
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_SelectIDCard")]
        public static extern int Syn_SelectIDCard(int iPort, ref byte[] pucIIN, int iIfOpen);
        [DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_ReadBaseMsg")]
        public static extern int Syn_ReadBaseMsg(int iPort, ref byte pucCHMsg, ref int puiCHMsgLen,ref byte pucPHMsg,ref int puiPHMsgLen, int iIfOpen);
        [DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_ReadBaseMsgToFile")]
        public static extern int Syn_ReadBaseMsgToFile(int iPort, ref char[] pcCHMsgFileName, ref int puiCHMsgLen, ref char[] pcPHMsgFileName, ref int puiPHMsgLen, int iIfOpen);
        [DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_ReadBaseFPMsg")]
        public static extern int Syn_ReadBaseFPMsg(int iPort, ref byte pucCHMsg, ref int puiCHMsgLen, ref byte pucPHMsg, ref int puiPHMsgLen, ref byte pucFPMsg, ref int puiFPMsgLen, int iIfOpen);
        [DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_ReadBaseFPMsgToFile")]
        public static extern int Syn_ReadBaseFPMsgToFile(int iPort, ref char[] pcCHMsgFileName, ref int puiCHMsgFileLen, ref char[] pcPHMsgFileName, ref int puiPHMsgFileLen, ref char[] pcFPMsgFileName, ref int puiFPMsgFileLen, int iIfOpen);
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_ReadNewAppMsg")]
        public static extern int Syn_ReadNewAppMsg(int iPort, ref byte pucAppMsg, ref int puiAppMsgLen, int iIfOpen);
        [DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_GetBmp")]
        public static extern int Syn_GetBmp(int iPort, ref char[] Wlt_File);
		/// <summary>
		/// 读取身份证中的基本信息和照片信息，并按设置转化信息和照片
		/// </summary>
		/// <param name="iPort">[in] 整数，表示端口号。参见Syn_ResetSAM</param>
		/// <param name="iIfOpen">[in] 整数，参见Syn_ResetSAM</param>
		/// <param name="pIDCardData">[out] IDCardData信息</param>
		/// <returns>0读取身份证信息成功，1读取身份证信息成功，解码照片不成功，其他.读取身份证信息失败</returns>
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_ReadMsg")]
        public static extern int Syn_ReadMsg(int iPort, int iIfOpen, ref IDCardData pIDCardData);
		/// <summary>
		/// 读取身份证中的基本信息和照片信息，并按设置转化信息和照片
		/// </summary>
		/// <param name="iPort">[in] 整数，表示端口号。参见Syn_ResetSAM</param>
		/// <param name="iIfOpen">[in] 整数，参见Syn_ResetSAM</param>
		/// <param name="pIDCardData">[out] IDCardData信息</param>
		/// <param name="cFPhotoname">cFPhotoName返回指纹信息文件名</param>
		/// <returns>0读取身份证信息成功，无指纹信息，1读取身份证信息成功，指纹读取成功，其他.读取身份证信息失败</returns>
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_ReadFPMsg")]
        public static extern int Syn_ReadFPMsg(int iPort, int iIfOpen, ref IDCardData pIDCardData, byte[] cFPhotoname);
		/// <summary>
		/// 自动寻找读卡器
		/// </summary>
		/// <returns>0失败，大于0为串口号或者端口号</returns>
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_FindReader")]
        public static extern int Syn_FindReader();
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_FindUSBReader")]
		public static extern int Syn_FindUSBReader();
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_BmpToJpeg")]
		public static extern int Syn_BmpToJpeg(ref char[] cBmpName, ref char[] cJpegName);
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_PhotoToStrBase64")]
		public static extern int Syn_PhotoToStrBase64(ref char[] cBase64, ref int iLen, ref char[] cPhotoName);
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_StrBase64ToPhoto")]
		public static extern int Syn_StrBase64ToPhoto(ref char[] cBase64, int iLen, ref char[] cPhotoName);

		//////////////////////////////////////////////////////////////////////////
		//				设置附加功能函数
		//
		//////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 设置照片文件存储的路径
		/// </summary>
		/// <param name="iOption">[in] 整形，0=C:根目录，	1=当前路径	，2=指定路径</param>
		/// <param name="cPhotoPath">[in] 字符指针。路径名</param>
		/// <returns></returns>
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_SetPhotoPath")]
		public static extern int Syn_SetPhotoPath(int iOption, ref byte[] cPhotoPath);
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_SetPhotoType")]
		public static extern int Syn_SetPhotoType(int iType);
		/// <summary>
		/// 设置照片文件的文件名
		/// </summary>
		/// <param name="iType">[in]整形。0=tmp，1=姓名，2=身份证号，3=姓名_身份证号</param>
		/// <returns>0成功，-1失败</returns>
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_SetPhotoName")]
		public static extern int Syn_SetPhotoName(int iType);
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_SetPhotoNameEx")]
		public static extern int Syn_SetPhotoNameEx(int iType, ref char[] cPhotoname);
		/// <summary>
		/// 设置返回性别的格式
		/// </summary>
		/// <param name="iType">[in] 整形。0=卡内存储的数据， 1=解释之后的数据</param>
		/// <returns>0成功，-1失败</returns>
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_SetSexType")]
		public static extern int Syn_SetSexType(int iType);
		/// <summary>
		/// 设置返回民族的格式
		/// </summary>
		/// <param name="iType">[in]整形。0=卡内存储的数据，1=解释之后的数据，2=解释之后+“族”</param>
		/// <returns>0成功，-1失败</returns>
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_SetNationType")]
		public static extern int Syn_SetNationType(int iType);
		/// <summary>
		/// 设置返回出生日期的格式
		/// </summary>
		/// <param name="iType">in] 整形。0=YYYYMMDD,1=YYYY年MM月DD日,2=YYYY.MM.DD,3=YYYY-MM-DD,4=YYYY/MM/DD</param>
		/// <returns>0成功，-1失败</returns>
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_SetBornType")]
		public static extern int Syn_SetBornType(int iType);
		/// <summary>
		/// 设置返回有效期开始日期的格式
		/// </summary>
		/// <param name="iType">[in] 整形。0=YYYYMMDD,1=YYYY年MM月DD日,2=YYYY.MM.DD,3=YYYY-MM-DD,4=YYYY/MM/DD</param>
		/// <returns>0成功，-1失败</returns>
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_SetUserLifeBType")]
		public static extern int Syn_SetUserLifeBType(int iType);
		/// <summary>
		/// 设置返回有效期结束日期的格式
		/// </summary>
		/// <param name="iType">[in] 整形。0=YYYYMMDD,1=YYYY年MM月DD日,2=YYYY.MM.DD,3=YYYY-MM-DD,4=YYYY/MM/DD</param>
		/// <param name="iOption">[in] 整形。0=长期不转换   1=长期转换为 有效期开始加50年</param>
		/// <returns>0成功，-1失败</returns>
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_SetUserLifeEType")]
		public static extern int Syn_SetUserLifeEType(int iType, int iOption);


		//////////////////////////////////////////////////////////////////////////
		//				USBHID函数
		//
		//////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 复位M1卡片，返回M1卡片的卡号和卡类型
		/// </summary>
		/// <param name="iPort">[in]整形，读卡器的端口号，可以使用Syn_FindReader获得</param>
		/// <param name="pdwCardSN">[out]无符号四字节，返回M1卡片卡号</param>
		/// <param name="pbSize">[out] 无符号字节数组，返回M1卡卡类型</param>
		/// <returns>0成功，其他失败</returns>
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_USBHIDM1Reset")]
		public static extern int Syn_USBHIDM1Reset(int iPort, ref UInt32 pdwCardSN, ref byte pbSize);
		/// <summary>
		/// 用密钥验证M1卡
		/// </summary>
		/// <param name="iPort">[in]整形，读卡器的端口号，可以使用Syn_FindReader获得</param>
		/// <param name="KeyType">[in]无符号字节，验证类型，0—密钥A验证 1—密钥B验证</param>
		/// <param name="BlockNo">[in]无符号字节，M1卡块号</param>
		/// <param name="pKey">[in]无符号字节数组，6字节，密钥数组</param>
		/// <returns>0成功，128失败</returns>
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_USBHIDM1AuthenKey")]
		public static extern int Syn_USBHIDM1AuthenKey(int iPort, byte KeyType, byte BlockNo, byte[] pKey);
		/// <summary>
		/// 读M1卡
		/// </summary>
		/// <param name="iPort">[in]整形，读卡器的端口号，可以使用Syn_FindReader获得</param>
		/// <param name="BlockNo">[in]无符号字节，M1卡块号</param>
		/// <param name="pBlock">[out]无符号字节数组，16字节，读取的数据</param>
		/// <returns>0成功，其他失败</returns>
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_USBHIDM1ReadBlock")]
		public static extern int Syn_USBHIDM1ReadBlock(int iPort, byte BlockNo, byte[] pBlock);
		/// <summary>
		/// 写M1卡
		/// </summary>
		/// <param name="iPort">[in]整形，读卡器的端口号，可以使用Syn_FindReader获得</param>
		/// <param name="BlockNo">[in]无符号字节，M1卡块号</param>
		/// <param name="pBlock">[in]无符号字节数组，16字节，读取的数据</param>
		/// <returns>0成功</returns>
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_USBHIDM1WriteBlock")]
		public static extern int Syn_USBHIDM1WriteBlock(int iPort, byte BlockNo, byte[] pBlock);
		/// <summary>
		/// 挂起M1卡
		/// </summary>
		/// <param name="iPort">[in]整形，读卡器的端口号，可以使用Syn_FindReader获得</param>
		/// <returns>0成功</returns>
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_USBHIDM1Halt")]
		public static extern int Syn_USBHIDM1Halt(int iPort);
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_USBHIDReadFactoryInfo")]
		public static extern int Syn_USBHIDReadFactoryInfo(int iPort, ref byte pFactory, ref byte pVol, ref byte pSerialNo, ref byte pReserved);
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_USBHIDWriteFactoryInfo")]
		public static extern int Syn_USBHIDWriteFactoryInfo(int iPort, ref byte pFactory, ref byte pVol, ref byte pSerialNo, ref byte pReserved);
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_USBHIDReadUserInfo")]
		public static extern int Syn_USBHIDReadUserInfo(int iPort, ref byte pInfo);
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_USBHIDWriteUserInfo")]
		public static extern int Syn_USBHIDWriteUserInfo(int iPort, ref byte pInfo);
		/// <summary>
		/// 获得读卡器支持的最大通讯字节数和版本
		/// </summary>
		/// <param name="iPort">[in]整形，读卡器的端口号，可以使用Syn_FindReader获得</param>
		/// <param name="pSAMMaxByte">[out]无符号字节，读卡器支持的最大通讯字节数</param>
		/// <param name="pARMVol">[out]无符号字节数组，版本号</param>
		/// <returns></returns>
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_USBHIDGetMaxByte")]
		public static extern int Syn_USBHIDGetMaxByte(int iPort, ref byte pSAMMaxByte, ref byte pARMVol);
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_USBHIDSendSound")]
		public static extern int Syn_USBHIDSendSound(int iPort,  byte ucSound);
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_USBHIDControlLED")]
		public static extern int Syn_USBHIDControlLED(int iPort, byte ucLed);
		/// <summary>
		/// 控制蜂鸣器命令（仅A16D-HF）
		/// </summary>
		/// <param name="iPort">[in]整形，读卡器的端口号，可以使用Syn_FindReader获得</param>
		/// <param name="ucType">[in]无符号整形，命令,暂定为0X04</param>
		/// <param name="usTime"> [in]整形，蜂鸣器响时间，毫秒计算</param>
		/// <returns>0成功</returns>
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_USBHIDControlBeep")]
		public static extern int Syn_USBHIDControlBeep(int iPort, byte ucType, int usTime);
		/// <summary>
		/// 读取二代证ID号（仅A16D-HF）
		/// </summary>
		/// <param name="iPort"> [in]整形，读卡器的端口号，可以使用Syn_FindReader获得</param>
		/// <param name="pdwCardSN"> [out] 无符号字节数组，长度8字节</param>
		/// <returns>0成功，128失败</returns>
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_USBHIDGetSecCardID")]
		public static extern int Syn_USBHIDGetSecCardID(int iPort, byte[] pdwCardSN);
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_USBHIDSIMpowerOn")]
		public static extern int Syn_USBHIDSIMpowerOn(int iPort, ref byte ucATR,ref byte ucLen);
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_USBHIDSIMpowerOff")]
		public static extern int Syn_USBHIDSIMpowerOff(int iPort);
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_USBHIDSIMAPDU")]
		public static extern int Syn_USBHIDSIMAPDU(int iPort, int slen, ref byte datasend, ref int rlen, ref byte datareceive);
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_USBHIDSetUserIDCode")]
		public static extern int Syn_USBHIDSetUserIDCode(int iPort, ref byte code, int len);
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_USBHIDSetUserIDCode")]
		public static extern int Syn_USBHIDWriteUserMac(int iPort, ref byte pMac);
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_USBHIDIsEncryptMode")]
		public static extern int Syn_USBHIDIsEncryptMode(int iPort);
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_USBHIDIsEncryptMode")]
		public static extern int Syn_USBHIDdoUpdate(int iPort, ref byte updateBuffer, int bufferLen, int progressCallback);

		//////////////////////////////////////////////////////////////////////////
		//				串口操作M1卡函数
		//
		//////////////////////////////////////////////////////////////////////////


        [DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_SerialFindReader")]
        public static extern int Syn_SerialFindReader(int iPort, ref int badurate);


		/// <summary>
		/// 打开串口
		/// </summary>
		/// <param name="iPort">串口号（1-16）</param>
		/// <param name="badurate">波特率（可使用Syn_GetCOMBaud获得）</param>
		/// <returns>0成功，其他失败</returns>
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_SerialOpen")]
		public static extern int Syn_SerialOpen(int iPort, int badurate);
		/// <summary>
		/// 关闭串口
		/// </summary>
		/// <param name="iPort">串口号（1-16）</param>
		/// <returns></returns>
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_SerialClose")]
		public static extern int Syn_SerialClose(int iPort);
		/// <summary>
		/// 复位M1卡片，返回M1卡片的卡号和卡类型
		/// </summary>
		/// <param name="iPort">[in]整形，读卡器的端口号，可以使用Syn_FindReader获得</param>
		/// <param name="pdwCardSN">[out]无符号四字节，返回M1卡片卡号</param>
		/// <param name="pbSize">[out] 无符号字节数组，返回M1卡卡类型</param>
		/// <returns>0成功，其他失败</returns>
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_SerialM1Reset")]
		public static extern int Syn_SerialM1Reset(int iPort, ref UInt32 pdwCardSN, ref byte pbSize);
		/// <summary>
		/// 用密钥验证M1卡
		/// </summary>
		/// <param name="iPort">[in]整形，读卡器的串口号（1-16），可以使用Syn_FindReader获得</param>
		/// <param name="KeyType">[in]无符号字节，验证类型，0—密钥A验证 1—密钥B验证</param>
		/// <param name="BlockNo">[in]无符号字节，M1卡块号</param>
		/// <param name="pKey">[in]无符号字节数组，6字节，密钥数组</param>
		/// <returns>0成功，128失败</returns>
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_SerialM1AuthenKey")]
		public static extern int Syn_SerialM1AuthenKey(int iPort,byte KeyType, byte BlockNo, byte[] pKey);
		/// <summary>
		/// 读M1卡
		/// </summary>
		/// <param name="iPort">[in]整形，读卡器的端口号，可以使用Syn_FindReader获得</param>
		/// <param name="BlockNo">[in]无符号字节，M1卡块号</param>
		/// <param name="pBlock">[out]无符号字节数组，16字节，读取的数据</param>
		/// <returns>0成功，其他失败</returns>
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_SerialM1ReadBlock")]
		public static extern int Syn_SerialM1ReadBlock(int iPort, byte BlockNo, byte[] pBlock);
		/// <summary>
		/// 写M1卡
		/// </summary>
		/// <param name="iPort">[in]整形，读卡器的端口号，可以使用Syn_FindReader获得</param>
		/// <param name="BlockNo">[in]无符号字节，M1卡块号</param>
		/// <param name="pBlock">[in]无符号字节数组，16字节，读取的数据</param>
		/// <returns>0成功</returns>
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_SerialM1WriteBlock")]
		public static extern int Syn_SerialM1WriteBlock(int iPort, byte BlockNo, byte[] pBlock);
		/// <summary>
		/// 挂起M1卡
		/// </summary>
		/// <param name="iPort">[in]整形，读卡器的端口号，可以使用Syn_FindReader获得</param>
		/// <returns>0成功</returns>
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_SerialM1Halt")]
		public static extern int Syn_SerialM1Halt(int iPort);
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_StartFindCPUCard")]
		public static extern int Syn_StartFindCPUCard(int iPort, byte[] ATS, ref int rlen);
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_StartRemoveCPUCard")]
		public static extern int Syn_StartRemoveCPUCard(int iPort, byte[] ATS);
		[DllImport("SynIDCardAPI.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "Syn_PCTOCPUCard")]
		public static extern int Syn_PCTOCPUCard(int iPort, int slen, byte[] datasend, ref int rlen, byte[] datareceive);



		/// <summary>
		/// 寻找USB读卡器
		/// </summary>
		/// <returns></returns>
		public static int findUSBDriverDevice()
		{
			int nRet;
			for (int i = 1001; i < 1017; i++)
			{
				nRet = ReadCardAPI.Syn_OpenPort(i);
				if (nRet == 0)
				{
					nRet = ReadCardAPI.Syn_GetSAMStatus(i, 0);
					if (nRet == 0)
					{
						ReadCardAPI.Syn_ClosePort(i);
						return i;
					}
					ReadCardAPI.Syn_ClosePort(i);
				}
			}
			return 0;
		}
		/// <summary>
		/// 寻找USBHID读卡器
		/// </summary>
		/// <returns></returns>
		public static int findUSBHIDDevice()
		{
			int nRet;
			nRet = Syn_OpenPort(9999);
			if (nRet == 0)
			{
				Syn_ClosePort(9999);
				return 9999;
			}
			return 0;
		}
		/// <summary>
		/// 寻找串口读卡器
		/// </summary>
		/// <returns></returns>
		public static int findSerialDevice()
		{
			int nRet = 0;
			int m_iBaud = 0;


            for (int k = 1; k < 50; k++)
            {
                nRet = Syn_SerialFindReader(k, ref m_iBaud);
                if (nRet == 0)
                {
                    return k;
                }
            }


			for (int j = 1; j < 17; j++)
			{
				nRet = Syn_GetCOMBaud(j, ref m_iBaud);
				if (nRet == 0)
				{
					nRet = Syn_SetCOMBaud(j, m_iBaud, m_iBaud);
					if (nRet == 0)
					{
						Syn_ClosePort(j);
						return j;
					}
					Syn_ClosePort(j);
				}
			}
			return 0;
		}

	}




	#region 结构体
	//[StructLayout(LayoutKind.Sequential)]
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
	public struct IDCardData
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string Name; //姓名   
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 6)]
		public string Sex;   //性别
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
		public string Nation; //名族
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 18)]
		public string Born; //出生日期
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 72)]
		public string Address; //住址
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 38)]
		public string IDCardNo; //身份证号
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string GrantDept; //发证机关
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 18)]
		public string UserLifeBegin; // 有效开始日期
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 18)]
		public string UserLifeEnd;  // 有效截止日期
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 38)]
		public string reserved; // 保留
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
		public string PhotoFileName; // 照片路径
	}

	#endregion


}
