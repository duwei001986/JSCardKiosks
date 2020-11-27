using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Management;
using System.Collections;

namespace JSCardKiosks
{
    class CRT_591_H001Help
    {
        [DllImport("CRT_591_H001.dll", EntryPoint = "CRT591H001ROpenWithBaut",CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr CRT591H001ROpenWithBaut(string port, uint baudrate);
        [DllImport("CRT_591_H001.dll", EntryPoint = "CRT591H001RClose",CallingConvention = CallingConvention.Cdecl)]
        public static extern int CRT591H001RClose(IntPtr ComHandle);
        [DllImport("CRT_591_H001.dll", EntryPoint = "RS232_ExeCommand",CallingConvention = CallingConvention.Cdecl)]
        public static extern int RS232_ExeCommand(IntPtr ComHandle, int TxDataLen, byte[] TxData, ref int RxDataLen, byte[] RxData);
       
    }
}
