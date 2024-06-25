using System.Runtime.InteropServices;

namespace AdmxPolicyManager.Interop
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct QueryServiceConfig
    {
        [MarshalAs(UnmanagedType.U4)]
        public int dwServiceType;

        [MarshalAs(UnmanagedType.U4)]
        public int dwStartType;

        [MarshalAs(UnmanagedType.U4)]
        public int dwErrorControl;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpBinaryPathName;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpLoadOrderGroup;

        [MarshalAs(UnmanagedType.U4)]
        public int dwTagId;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpDependencies;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpServiceStartName;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpDisplayName;
    }
}
