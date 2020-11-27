using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace JSCardKiosks
{
    class PrintControl
    {
        public const UInt64 S7PS_S_DETECTCARD = 0x0000000002000000;
        [DllImport("ZJ70Control.dll", CharSet = CharSet.Ansi)]
        public static extern int GetPrinterStatus(byte mdoCurr, out UInt64 status);

        [DllImport("ZJ70Control.dll", CharSet = CharSet.Ansi)]
        public static extern int IDPCardKeep(int distace = 650);

        [DllImport("ZJ70Control.dll", CharSet = CharSet.Ansi)]
        public static extern int IDPGetRibbonRemain(byte mdoCurr, out int pnType, out int pnRemain, out int pnMax, out UInt64 status);

        [DllImport("ZJ70Control.dll", CharSet = CharSet.Ansi)]
        public static extern int IDPCardOut();

        [DllImport("ZJ70Control.dll", CharSet = CharSet.Ansi)]
        public static extern int ExSetICPosConfig(int distance);

        [DllImport("ZJ70Control.dll", EntryPoint = "IDPCardPrint", CharSet = CharSet.Unicode)]
        public static extern int IDPCardPrint(string strName, string strIDNum, string strCardNum, string strCardDate, string strPhotoPath, string configPath);
        public static int IsHasCardInPrinter()
        {
            UInt64 status = 0;
            int iRet = GetPrinterStatus(0x40, out status);
            if (iRet != 0)
                return iRet;
            for (int i = 0; i < 64; i++)
            {
                UInt64 status_flag = status & ((UInt64)1 << i);
                switch (status_flag)
                {
                    case S7PS_S_DETECTCARD:
                        {
                            return 1;
                        }
                    case 0: break;
                    default: break;
                }
            }
            return 0;
        }
        public static int checkPrinterStatus(out bool isHasCard,out int ribbonCount)
        {
            int pnType = 0, pnRemain = 0, pnMax = 0;
            UInt64 status = 0;
            isHasCard = false;
            ribbonCount = 0;
            int iRet = IDPGetRibbonRemain(0x40, out pnType, out pnRemain, out pnMax, out status);
            if (iRet!= 0)
                return iRet;
            ribbonCount = pnRemain;
            for (int i = 0; i < 64; i++)
            {
                UInt64 status_flag = status & ((UInt64)1 << i);
                switch (status_flag)
                {
                    case S7PS_S_DETECTCARD:
                        {
                            isHasCard =  true;
                            return 0;
                        }
                    case 0: break;
                    default: break;
                }
            }
            return iRet;
        }
    }
}
