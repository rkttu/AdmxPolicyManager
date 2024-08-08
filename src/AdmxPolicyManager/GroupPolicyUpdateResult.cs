namespace AdmxPolicyManager;

/// <summary>
/// Represents the result of a group policy update operation.
/// </summary>
public enum GroupPolicyUpdateResult
{
    /// <summary>
    /// The group policy update operation succeeded.
    /// </summary>
    UpdateSucceed = 0,

    /// <summary>
    /// Failed to create or open the group policy.
    /// </summary>
    CreateOrOpenFailed,

    /// <summary>
    /// Failed to save the group policy.
    /// </summary>
    SaveFailed,

    /// <summary>
    /// Failed to set the group policy.
    /// </summary>
    SetFailed,

    /// <summary>
    /// Before the group policy is updated.
    /// </summary>
    BeforeUpdate,
}
