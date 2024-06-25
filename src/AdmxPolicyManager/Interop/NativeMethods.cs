using System;
using System.Runtime.InteropServices;
using System.Text;

namespace AdmxPolicyManager.Interop
{
    internal static class NativeMethods
    {
        public static readonly int S_OK = 0x00000000;

        public static readonly Guid REGISTRY_EXTENSION_GUID = new Guid(
            0x35378EAC, 0x683F, 0x11D2, 0xA8, 0x9A, 0x00, 0xC0, 0x4F, 0xBB, 0xCF, 0xA2);

        public static readonly Guid ADMXCOMMENTS_EXTENSION_GUID = new Guid(
            0x6C5A2A86, 0x9EB3, 0x42b9, 0xAA, 0x83, 0xA7, 0x37, 0x1B, 0xA0, 0x11, 0xB9);

        [DllImport("userenv.dll", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        public static extern SafeCriticalPolicySectionHandle EnterCriticalPolicySection(
            [MarshalAs(UnmanagedType.Bool)] bool bMachinePolicy);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("userenv.dll", CharSet = CharSet.None, SetLastError = true, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool RefreshPolicy(
            [MarshalAs(UnmanagedType.Bool)] bool bMachine);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("userenv.dll", CharSet = CharSet.None, SetLastError = true, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
        public static extern bool RefreshPolicyEx(
            [MarshalAs(UnmanagedType.Bool)] bool bMachine,
            [MarshalAs(UnmanagedType.U4)] int dwOptions);

        public static readonly int RP_FORCE = 1;

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern SafeServiceHandle OpenSCManager(
           string lpMachineName,
           string lpDatabaseName,
           [MarshalAs(UnmanagedType.U4)] int dwDesiredAccess);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern SafeServiceHandle OpenServiceW(
            SafeServiceHandle hSCManager,
            string lpServiceName,
            [MarshalAs(UnmanagedType.U4)] int dwDesiredAccess);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool QueryServiceStatus(
            SafeServiceHandle hService,
            out ServiceStatus lpServiceStatus);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern bool QueryServiceConfigW(
            SafeServiceHandle hService,
            IntPtr lpServiceConfig,
            [MarshalAs(UnmanagedType.U4)] int cbBufSize,
            [MarshalAs(UnmanagedType.U4)] out int pcbBytesNeeded);

        public const int SC_MANAGER_ENUMERATE_SERVICE = 0x0004;
        public const int SERVICE_QUERY_CONFIG = 0x0001;
        public const int SERVICE_QUERY_STATUS = 0x0004;

        public const int SERVICE_STOPPED = 1;
        public const int SERVICE_START_PENDING = 2;
        public const int SERVICE_STOP_PENDING = 3;
        public const int SERVICE_RUNNING = 4;
        public const int SERVICE_CONTINUE_PENDING = 5;
        public const int SERVICE_PAUSE_PENDING = 6;
        public const int SERVICE_PAUSED = 7;

        public const int SERVICE_BOOT_START = 0x00000000;
        public const int SERVICE_SYSTEM_START = 0x00000001;
        public const int SERVICE_AUTO_START = 0x00000002;
        public const int SERVICE_DEMAND_START = 0x00000003;
        public const int SERVICE_DISABLED = 0x00000004;

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool LookupAccountSidW(
           string lpSystemName,
           [MarshalAs(UnmanagedType.LPArray)] byte[] Sid,
           StringBuilder Name,
           [MarshalAs(UnmanagedType.U4)] ref int cchName,
           StringBuilder ReferencedDomainName,
           [MarshalAs(UnmanagedType.U4)] ref int cchReferencedDomainName,
           [MarshalAs(UnmanagedType.U4)] out SidNameUsage peUse);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ConvertStringSidToSidW(
            string StringSid,
            out IntPtr Sid);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.U4)]
        public static extern int GetLengthSid(IntPtr pSid);

        [DllImport("advapi32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi, CharSet = CharSet.None, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CreateWellKnownSid(
            int WellKnownSidType,
            IntPtr DomainSid,
            byte[] pSid,
            [MarshalAs(UnmanagedType.U4)] ref int cbSid);

        [DllImport("advapi32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi, CharSet = CharSet.None, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ConvertSidToStringSidW(
            IntPtr Sid,
            out IntPtr StringSid);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LocalFree(IntPtr hMem);
    }
}
