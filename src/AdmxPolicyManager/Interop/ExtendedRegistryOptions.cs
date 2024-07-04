using System;

namespace AdmxPolicyManager.Interop;

[Flags]
internal enum ExtendedRegistryOptions : int
{
    NonVolatile = 0x0,
    Volatile = 0x1,
    CreateLink = 0x2,
    BackupRestore = 0x4,
    OpenLink = 0x8
}
