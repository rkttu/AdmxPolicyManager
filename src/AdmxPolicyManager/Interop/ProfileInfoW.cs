using System;
using System.Runtime.InteropServices;

namespace AdmxPolicyManager.Interop
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct ProfileInfoW
    {
        [MarshalAs(UnmanagedType.U4)]
        public int dwSize;

        [MarshalAs(UnmanagedType.U4)]
        public int dwFlags;

        public string lpUserName;
        public string lpProfilePath;
        public string lpDefaultPath;
        public string lpServerName;
        public string lpPolicyPath;
        public IntPtr hProfile;
    }
}
