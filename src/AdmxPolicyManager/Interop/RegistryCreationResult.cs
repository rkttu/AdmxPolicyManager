namespace AdmxPolicyManager.Interop;

/// <summary>
/// Flag returned by calling RegCreateKeyEx.
/// </summary>
internal enum RegistryCreationResult : int
{
    CreatedNewKey = 0x0,
    OpenedExistingKey = 0x1,
}
