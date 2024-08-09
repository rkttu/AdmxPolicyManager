using AdmxPolicyManager.Interop;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace AdmxPolicyManager;

internal static class NativeMethods
{
    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    public static extern int RegOpenKeyExW(
        IntPtr hKey,
        string lpSubKey,
        [MarshalAs(UnmanagedType.U4)] ExtendedRegistryOptions ulOptions,
        DesiredSAMPermissions samDesired,
        out IntPtr phkResult);

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    public static extern int RegEnumValueW(
        IntPtr hKey,
        [MarshalAs(UnmanagedType.U4)] int dwIndex,
        StringBuilder lpValueName,
        [MarshalAs(UnmanagedType.U4)] ref int lpcchValueName,
        IntPtr lpReserved,
        [Out] IntPtr lpType,
        [Out] IntPtr lpData,
        [In, Out] IntPtr lpcbData);

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    public static extern int RegQueryValueExW(
        IntPtr hKey,
        string lpValueName,
        IntPtr lpReserved,
        [MarshalAs(UnmanagedType.U4)] out RegistryValueType lpType,
        [Out] byte[] lpData,
        [MarshalAs(UnmanagedType.U4)] ref int lpcbData);

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    public static extern int RegSetValueExW(
        IntPtr hKey,
        string lpValueName,
        [MarshalAs(UnmanagedType.U4)] int Reserved,
        [MarshalAs(UnmanagedType.U4)] RegistryValueType dwType,
        IntPtr lpData,
        [MarshalAs(UnmanagedType.U4)] int cbData);

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    public static extern int RegCreateKeyExW(
        IntPtr hKey,
        string lpSubKey,
        [MarshalAs(UnmanagedType.U4)] int Reserved,
        string? lpClass,
        [MarshalAs(UnmanagedType.U4)] ExtendedRegistryOptions dwOptions,
        [MarshalAs(UnmanagedType.U4)] DesiredSAMPermissions samDesired,
        IntPtr lpSecurityAttributes,
        out IntPtr phkResult,
        [MarshalAs(UnmanagedType.U4)] out RegistryCreationResult lpdwDisposition);

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    public static extern int RegDeleteKeyExW(
        IntPtr hKey,
        string lpSubKey,
        DesiredSAMPermissions samDesired,
        [MarshalAs(UnmanagedType.U4)] int Reserved);

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    public static extern int RegDeleteValueW(
        IntPtr hKey,
        string lpValueName);

    [DllImport("advapi32.dll", CharSet = CharSet.None, ExactSpelling = true)]
    public static extern int RegCloseKey(IntPtr hKey);

    public static readonly int S_OK = 0x00000000;

    public static readonly int ERROR_SUCCESS = 0;
    public static readonly int ERROR_MORE_DATA = 234;
    public static readonly int ERROR_NO_MORE_ITEMS = 259;
    public static readonly int ERROR_SHARING_VIOLATION = unchecked((int)0x80070020u);

    public static readonly int MAX_REG_VALUE_NAME_SIZE = 16383;

    public static readonly Guid REGISTRY_EXTENSION_GUID =
        new Guid("35378EAC-683F-11D2-A89A-00C04FBBCFA2");

    public static readonly Guid CLSID_GPESnapIn =
        new Guid("8FC0B734-A0E1-11d1-A7D3-0000F87571E3");

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

    [DllImport("userenv.dll", CharSet = CharSet.None, SetLastError = true, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    public static extern IntPtr EnterCriticalPolicySection(
        [MarshalAs(UnmanagedType.Bool)] bool bMachine);

    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("userenv.dll", CharSet = CharSet.None, SetLastError = true, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    public static extern bool LeaveCriticalPolicySection(
        IntPtr hSection);

    public static int AssertResultCode(this int resultCode)
    {
        if (resultCode == NativeMethods.S_OK)
            return resultCode;

        throw new Win32Exception(resultCode);
    }

    public static bool IsSuccessCode(this int resultCode)
        => resultCode == NativeMethods.S_OK;

    public static bool IsNotSuccessCode(this int resultCode)
        => resultCode != NativeMethods.S_OK;

    public static bool IsHasMoreDataCode(this int resultCode)
        => resultCode == NativeMethods.ERROR_MORE_DATA;

    public static bool IsErrorNoMoreItemsCode(this int resultCode)
        => resultCode == NativeMethods.ERROR_NO_MORE_ITEMS;
}
