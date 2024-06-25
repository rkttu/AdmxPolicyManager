using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

namespace AdmxPolicyManager.Interop
{
    internal sealed class SafeServiceHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public SafeServiceHandle()
            : base(true) { }

        protected override bool ReleaseHandle()
            => CloseServiceHandle(this.handle);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseServiceHandle(IntPtr hSCObject);
    }
}
