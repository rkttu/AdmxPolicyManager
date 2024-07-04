using System;
using System.Runtime.InteropServices;
using System.Text;

namespace AdmxPolicyManager.Interop;

[ComImport]
[Guid("7E37D5E7-263D-45CF-842B-96A95C63E46C")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IGroupPolicyObject2
{
    [return: MarshalAs(UnmanagedType.U4)]
    int New(
        [MarshalAs(UnmanagedType.LPWStr)] string pszDomainName,
        [MarshalAs(UnmanagedType.LPWStr)] string pszDisplayName,
        [MarshalAs(UnmanagedType.U4)] int dwFlags);

    [return: MarshalAs(UnmanagedType.U4)]
    int OpenDSGPO(
        [MarshalAs(UnmanagedType.LPWStr)] string pszPath,
        [MarshalAs(UnmanagedType.U4)] int dwFlags);

    [return: MarshalAs(UnmanagedType.U4)]
    int OpenLocalMachineGPO(
        [MarshalAs(UnmanagedType.U4)] int dwFlags);

    [return: MarshalAs(UnmanagedType.U4)]
    int OpenRemoteMachineGPO(
        [MarshalAs(UnmanagedType.LPWStr)] string pszComputerName,
        [MarshalAs(UnmanagedType.U4)] int dwFlags);

    [return: MarshalAs(UnmanagedType.U4)]
    int Save(
        [MarshalAs(UnmanagedType.Bool)] bool bMachine,
        [MarshalAs(UnmanagedType.Bool)] bool bAdd,
        [MarshalAs(UnmanagedType.LPStruct)] Guid pGuidExtension,
        [MarshalAs(UnmanagedType.LPStruct)] Guid pGuid);

    [return: MarshalAs(UnmanagedType.U4)]
    int Delete();

    [return: MarshalAs(UnmanagedType.U4)]
    int GetName(
        [MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName,
        int cchMaxLength);

    [return: MarshalAs(UnmanagedType.U4)]
    int GetDisplayName(
        [MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName,
        int cchMaxLength);

    [return: MarshalAs(UnmanagedType.U4)]
    int SetDisplayName(
        [MarshalAs(UnmanagedType.LPWStr)] string pszName);

    [return: MarshalAs(UnmanagedType.U4)]
    int GetPath(
        [MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszPath,
        int cchMaxPath);

    [return: MarshalAs(UnmanagedType.U4)]
    int GetDSPath(
        [MarshalAs(UnmanagedType.U4)] int dwSection,
        [MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszPath,
        int cchMaxPath);

    [return: MarshalAs(UnmanagedType.U4)]
    int GetFileSysPath(
        [MarshalAs(UnmanagedType.U4)] int dwSection,
        [MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszPath,
        int cchMaxPath);

    [return: MarshalAs(UnmanagedType.U4)]
    int GetRegistryKey(
        [MarshalAs(UnmanagedType.U4)] int dwSection,
        out IntPtr hKey);

    [return: MarshalAs(UnmanagedType.U4)]
    int GetOptions(
        [MarshalAs(UnmanagedType.U4)] out int dwOptions);

    [return: MarshalAs(UnmanagedType.U4)]
    int SetOptions(
        [MarshalAs(UnmanagedType.U4)] int dwOptions,
        [MarshalAs(UnmanagedType.U4)] int dwMask);

    [return: MarshalAs(UnmanagedType.U4)]
    int GetType(
        out IntPtr gpoType);

    [return: MarshalAs(UnmanagedType.U4)]
    int GetMachineName(
        [MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName,
        int cchMaxLength);

    [return: MarshalAs(UnmanagedType.U4)]
    int GetPropertySheetPages(
        out IntPtr hPages,
        [MarshalAs(UnmanagedType.U4)] out int uPageCount);

    [return: MarshalAs(UnmanagedType.U4)]
    int OpenLocalMachineGPOForPrincipal(
        [MarshalAs(UnmanagedType.LPWStr)] string pszLocalUserOrGroupSID,
        [MarshalAs(UnmanagedType.U4)] int dwFlags);

    [return: MarshalAs(UnmanagedType.U4)]
    int GetRegistryKeyPath(
        [MarshalAs(UnmanagedType.U4)] int dwSection,
        [MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszRegistryKeyPath,
        int cchMaxLength);
}
