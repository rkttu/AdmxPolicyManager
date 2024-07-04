using System;
using System.Runtime.InteropServices;

namespace AdmxPolicyManager.Interop;

/// <summary>
/// Structure for security attributes.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct SecurityAttributes
{
    public int nLength;

    public IntPtr lpSecurityDescriptor;

    [MarshalAs(UnmanagedType.Bool)]
    public bool bInheritHandle;
}
