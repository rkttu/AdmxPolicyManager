using System;

namespace AdmxPolicyManager.Interop;

[Flags]
internal enum DesiredSAMPermissions : int
{
    None = 0x00000000,
    QueryValue = 0x00000001,
    SetValue = 0x00000002,
    CreateSubKey = 0x00000004,
    EnumerateSubKeys = 0x00000008,
    Notify = 0x00000010,
    CreateLink = 0x00000020,
    WOW64_32Key = 0x00000200,
    WOW64_64Key = 0x00000100,
    WOW64_Res = 0x00000300,
    Read = 0x00020019,
    Write = 0x00020006,
    Execute = 0x00020019,
    AllAccess = 0x000f003f,
}
