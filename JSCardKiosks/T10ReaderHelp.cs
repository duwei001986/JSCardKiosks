using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CardWriteReadT10
{
    class T10ReaderHelp
    {
        [DllImport("dcrf32.dll")]
        public static extern int dc_init(Int16 port, Int32 baud);  //初试化
        [DllImport("dcrf32.dll")]
        public static extern short dc_exit(int icdev);
        [DllImport("dcrf32.dll")]
        public static extern short dc_beep(int icdev, ushort misc);
        [DllImport("dcrf32.dll")]
        public static extern short dc_reset(int icdev, uint sec);
        [DllImport("dcrf32.dll")]
        public static extern short dc_config_card(int icdev, byte cardType);
        [DllImport("dcrf32.dll")]

        public static extern short dc_card(int icdev, byte model, ref ulong snr);

        [DllImport("dcrf32.dll")]
        public static extern short dc_card_double(int icdev, byte model, [Out] byte[] snr);

        [DllImport("dcrf32.dll")]
        public static extern short dc_card_double_hex(int icdev, byte model, [Out]char[] snr);

        [DllImport("dcrf32.dll")]
        public static extern short dc_pro_reset(int icdev, ref byte rlen, [Out] byte[] recvbuff);
        [DllImport("dcrf32.dll")]
        public static extern short dc_pro_command(int icdev, byte slen, byte[] sendbuff, ref byte rlen,
                                                     [Out]byte[] recvbuff, byte timeout);
        [DllImport("dcrf32.dll")]
        public static extern short dc_card_b(int icdev, [Out] byte[] atqb);


        [DllImport("dcrf32.dll")]
        public static extern short dc_setcpu(int icdev, byte address);
        [DllImport("dcrf32.dll")]
        public static extern short dc_cpureset(int icdev, ref byte rlen, byte[] rdata);
        [DllImport("dcrf32.dll")]
        public static extern short dc_cpuapdu(int icdev, byte slen, byte[] sendbuff, ref byte rlen,
                                                     [Out]byte[] recvbuff);

        [DllImport("dcrf32.dll")]
        public static extern short dc_readpincount_4442(int icdev);
        [DllImport("dcrf32.dll")]
        public static extern short dc_read_4442(int icdev, Int16 offset, Int16 lenth, byte[] buffer);
        [DllImport("dcrf32.dll")]
        public static extern short dc_write_4442(int icdev, Int16 offset, Int16 lenth, byte[] buffer);
        [DllImport("dcrf32.dll")]
        public static extern short dc_verifypin_4442(int icdev, byte[] password);
        [DllImport("dcrf32.dll")]
        public static extern short dc_readpin_4442(int icdev, byte[] password);
        [DllImport("dcrf32.dll")]
        public static extern short dc_changepin_4442(int icdev, byte[] password);

        [DllImport("dcrf32.dll")]
        public static extern short dc_readpincount_4428(int icdev);
        [DllImport("dcrf32.dll")]
        public static extern short dc_read_4428(int icdev, Int16 offset, Int16 lenth, byte[] buffer);
        [DllImport("dcrf32.dll")]
        public static extern short dc_write_4428(int icdev, Int16 offset, Int16 lenth, byte[] buffer);
        [DllImport("dcrf32.dll")]
        public static extern short dc_verifypin_4428(int icdev, byte[] password);
        [DllImport("dcrf32.dll")]
        public static extern short dc_readpin_4428(int icdev, byte[] password);
        [DllImport("dcrf32.dll")]
        public static extern short dc_changepin_4428(int icdev, byte[] password);

        [DllImport("dcrf32.dll")]
        public static extern int dc_authentication(int icdev, int _Mode, int _SecNr);

        [DllImport("dcrf32.dll")]
        public static extern int dc_authentication_pass(int icdev, int _Mode, int _SecNr, byte[] nkey);

        [DllImport("dcrf32.dll")]
        public static extern int dc_authentication_pass_hex(int icdev, int _Mode, int _SecNr, string nkey);

        [DllImport("dcrf32.dll")]
        public static extern int dc_load_key(int icdev, int mode, int secnr, byte[] nkey);  //密码装载到读写模块中


        [DllImport("dcrf32.dll")]
        public static extern int dc_write(int icdev, int adr, [In] byte[] sdata);  //向卡中写入数据

        [DllImport("dcrf32.dll")]
        public static extern int dc_write_hex(int icdev, int adr, [In] string sdata);  //向卡中写入数据


        [DllImport("dcrf32.dll")]
        public static extern int dc_read(int icdev, int adr, [Out] byte[] sdata);  //从卡中读数据

        [DllImport("dcrf32.dll")]
        public static extern short dc_read_24c(int icdev, Int16 offset, Int16 lenth, byte[] buffer);
        [DllImport("dcrf32.dll")]
        public static extern short dc_write_24c(int icdev, Int16 offset, Int16 lenth, byte[] buffer);

        [DllImport("dcrf32.dll")]
        public static extern short dc_read_24c64(int icdev, Int16 offset, Int16 lenth, byte[] buffer);
        [DllImport("dcrf32.dll")]
        public static extern short dc_write_24c64(int icdev, Int16 offset, Int16 lenth, byte[] buffer);


    }
}
