using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace JSCardKiosks
{
    class IDP51PrintControl
    {
        public const UInt64 S51PS_M_CARDIN = 0x0000000000000001;	// inserting card
        public const UInt64 S51PS_M_CARDMOVE = 0x0000000000000002;	// moving card
        public const UInt64 S51PS_M_CARDMOVEEXT = 0x0000000000000004;	// moving card between external
        public const UInt64 S51PS_M_CARDEJECT = 0x0000000000000008;	// ejecting card
        public const UInt64 S51PS_M_THEADLIFT = 0x0000000000000010;	// lifting up/down thermal head
        public const UInt64 S51PS_M_ICLIFT = 0x0000000000000020;	// lifting up/down ic connector
        public const UInt64 S51PS_M_RIBBONSEARCH = 0x0000000000000040;	// searching ribbon
        public const UInt64 S51PS_M_RIBBONWIND = 0x0000000000000080;	// winding ribbon
        public const UInt64 S51PS_M_MAGNETIC = 0x0000000000000100;	// processing magnetic
        public const UInt64 S51PS_M_PRINT = 0x0000000000000200;	// printing
        public const UInt64 S51PS_M_INIT = 0x0000000000000400;	// initializing
        public const UInt64 S51PS_S_CONNHOPPER = 0x0000000000000800;	// hopper connected
        public const UInt64 S51PS_S_CONNICENCODEER = 0x0000000000001000;	// ic encoder connected
        public const UInt64 S51PS_S_CONNMAGNETIC = 0x0000000000002000;	// magnetic encoder connected
        public const UInt64 S51PS_S_CONNLAMINATOR = 0x0000000000004000;	// laminator connected
        public const UInt64 S51PS_S_CONNFLIPPER = 0x0000000000008000;	// flipper connected
        public const UInt64 S51PS_S_FLIPPERTOP = 0x0000000000010000;	// flipper is top sided
        public const UInt64 S51PS_S_COVEROPENED = 0x0000000000020000;	// cover is opened
        public const UInt64 S51PS_S_DETECTIN = 0x0000000000040000;	// detect a card from in sensor
        public const UInt64 S51PS_S_DETECTOUT = 0x0000000000080000;	// detect a card from out sensor
        public const UInt64 S51PS_S_CARDEMPTY = 0x0000000000100000;	// card empty
        public const UInt64 S51PS_S_RECVPRINTDATA = 0x0000000000200000;	// receiving print data
        public const UInt64 S51PS_S_HAVEPRINTDATA = 0x0000000000400000;	// having print data
        public const UInt64 S51PS_S_NEEDCLEANING = 0x0000000004000000;	// need cleaning
        public const UInt64 S51PS_S_SWLOCKED = 0x0000000008000000;	// system locked (sw)
        public const UInt64 S51PS_S_HWLOCKED = 0x0000000010000000;	// system locked (hw)
        public const UInt64 S51PS_M_SBSCOMMAND = 0x0000000020000000;	// doing SBS command
        public const UInt64 S51PS_S_SBSMODE = 0x0000000040000000;	// under SBS mode
        public const UInt64 S51PS_S_TESTMODE = 0x0000000080000000;  // test mode

        // ERROR-PART
        public const UInt64 S51PS_F_CARDIN = 0x0000000100000000;	// failed to insert card
        public const UInt64 S51PS_F_CARDMOVE = 0x0000000200000000;	// failed to move card
        public const UInt64 S51PS_F_CARDMOVEEXT = 0x0000000400000000;	// failed to move card between external
        public const UInt64 S51PS_F_CARDEJECT = 0x0000000800000000;	// failed to eject card
        public const UInt64 S51PS_F_THEADLIFT = 0x0000001000000000;	// failed to lift up/down thermal head
        public const UInt64 S51PS_F_ICLIFT = 0x0000002000000000;	// failed to lift up/down ic connector
        public const UInt64 S51PS_F_RIBBONSEARCH = 0x0000004000000000;	// failed to search ribbon
        public const UInt64 S51PS_F_RIBBONWIND = 0x0000008000000000;	// failed to wind ribbon
        public const UInt64 S51PS_F_MAGNETIC = 0x0000010000000000;	// failed to read/write magnetic
        public const UInt64 S51PS_F_READMAGT1 = 0x0000020000000000;	// failed to read magnetic track 1
        public const UInt64 S51PS_F_READMAGT2 = 0x0000040000000000;	// failed to read magnetic track 2
        public const UInt64 S51PS_F_READMAGT3 = 0x0000080000000000;	// failed to read magnetic track 3
        public const UInt64 S51PS_F_PRINT = 0x0000100000000000;	// error from printing
        public const UInt64 S51PS_E_INIT = 0x0000200000000000;	// error from initializing
        public const UInt64 S51PS_E_CONNEXT = 0x0000400000000000;	// error from device connection -failed to connect
        public const UInt64 S51PS_E_CONNLAMINATOR = 0x0000800000000000;	// error from device connection -laminator
        public const UInt64 S51PS_E_CONNFLIPPER = 0x0001000000000000;	// error from device connection -flipper
        public const UInt64 S51PS_E_RIBBON0 = 0x0020000000000000;	// ribbon remain 0
        public const UInt64 S51PS_E_NORIBBON = 0x0040000000000000;	// no ribbon
        public const UInt64 S51PS_E_NOTHEAD = 0x0080000000000000;	// no thermal head
        public const UInt64 S51PS_E_OVERHEAT = 0x0100000000000000;	// thermal head overheat
        public const UInt64 S51PS_F_INVALIDPRINTDATA = 0x0200000000000000;	// invalid printing data format
        public const UInt64 S51PS_F_INVALIDPASSWORD = 0x0400000000000000;	// invalid password
        public const UInt64 S51PS_F_SET = 0x4000000000000000;	// failed to set
        public const UInt64 S51PS_F_SPOOLFULL = 0x8000000000000000;	// fulled spool pool
        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetDefaultPrinter(string pszPrinter);
        [DllImport("51PrintDll.dll", EntryPoint = "IDPGetRibbonRemain", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int IDPGetRibbonRemain(out int pnRemain, out UInt64 status);
        [DllImport("51PrintDll.dll", EntryPoint = "GetPrinterStatus", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetPrinterStatus(ref UInt64 status);
        [DllImport("51PrintDll.dll", EntryPoint = "IDPCardIn", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int IDPCardIn();
        [DllImport("51PrintDll.dll", EntryPoint = "IDP51Reboot", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int IDP51Reboot();
        [DllImport("51PrintDll.dll", EntryPoint = "IDPCardOut", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int IDPCardOut();
        [DllImport("51PrintDll.dll", EntryPoint = "IDPCardReBack", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int IDPCardReBack(int distance);

        [DllImport("51PrintDll.dll", EntryPoint = "IDPCardPrint", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int IDPCardPrint(string printLongText);
        public static int checkPrinterStatus(out bool isHasCard, out int ribbonCount)
        {
            int  pnRemain = 0;
            UInt64 status = 0;
            isHasCard = false;
            ribbonCount = 0;
            int iRet = IDPGetRibbonRemain(out pnRemain, out status);
            if (iRet != 0)
                return iRet;
            ribbonCount = pnRemain;
            for (int i = 0; i < 64; i++)
            {
                UInt64 status_flag = status & ((UInt64)1 << i);
                switch (status_flag)
                {
                    case S51PS_S_DETECTOUT:
                        {
                            isHasCard = true;
                            return 0;
                        }
                    case S51PS_S_DETECTIN:
                        {
                            isHasCard = true;
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
