namespace AdmxPolicyManager;

/// <summary>
/// Represents the result of a group policy delete operation.
/// </summary>
public enum GroupPolicyDeleteResult
{
    /// <summary>
    /// The group policy delete operation succeeded.
    /// </summary>
    DeleteSucceed = 0,

    /// <summary>
    /// No item found to delete.
    /// </summary>
    NoItemFound,

    /// <summary>
    /// Failed to create or open the group policy.
    /// </summary>
    CreateOrOpenFailed,

    /// <summary>
    /// Failed to save the group policy.
    /// </summary>
    SaveFailed,
}
