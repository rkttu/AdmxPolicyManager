namespace AdmxPolicyManager.Interop;

/// <summary>
/// Group Policy Object type.
/// </summary>
internal enum GroupPolicyObjectType : int
{
    /// <summary>
    /// Default GPO on the local machine
    /// </summary>
    Local = 0,

    /// <summary>
    /// GPO on a remote machine
    /// </summary>
    Remote,

    /// <summary>
    /// GPO in the Active Directory
    /// </summary>
    ActiveDirectory,

    /// <summary>
    /// User-specific GPO on the local machine
    /// </summary>
    LocalUser,

    /// <summary>
    /// Group-specific GPO on the local machine
    /// </summary>
    LocalGroup,
}