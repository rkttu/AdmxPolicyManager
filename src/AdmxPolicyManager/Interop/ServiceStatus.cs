using System.Runtime.InteropServices;

namespace AdmxPolicyManager.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct ServiceStatus
    {
        [MarshalAs(UnmanagedType.U4)]
        public int dwServiceType;

        [MarshalAs(UnmanagedType.U4)]
        public int dwCurrentState;

        [MarshalAs(UnmanagedType.U4)]
        public int dwControlsAccepted;

        [MarshalAs(UnmanagedType.U4)]
        public int dwWin32ExitCode;

        [MarshalAs(UnmanagedType.U4)]
        public int dwServiceSpecificExitCode;

        [MarshalAs(UnmanagedType.U4)]
        public int dwCheckPoint;

        [MarshalAs(UnmanagedType.U4)]
        public int dwWaitHint;
    }
}
