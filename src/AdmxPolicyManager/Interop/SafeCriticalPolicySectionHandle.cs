using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

namespace AdmxPolicyManager.Interop
{
    internal sealed class SafeCriticalPolicySectionHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public SafeCriticalPolicySectionHandle()
             : base(true) { }

        protected override bool ReleaseHandle()
            => LeaveCriticalPolicySection(handle);

        [DllImport("userenv.dll", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool LeaveCriticalPolicySection(
            IntPtr hSection);
    }
}
