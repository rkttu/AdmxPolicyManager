namespace AdmxPolicyManager.Interop;

/// <summary>
/// Group Policy Object open and creation flags
/// </summary>
internal enum GroupPolicyOpenFlags : int
{
    /// <summary>
    /// Load the registry files
    /// </summary>
    LoadRegistry = 0x00000001,

    /// <summary>
    /// Open the GPO as read only
    /// </summary>
    ReadOnly = 0x00000002,
}
