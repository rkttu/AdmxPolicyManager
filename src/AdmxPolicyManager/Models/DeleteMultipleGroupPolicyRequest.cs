namespace AdmxPolicyManager.Models;

/// <summary>
/// Request model for deleting multiple group policies.
/// </summary>
public sealed class DeleteMultipleGroupPolicyRequest
{
    /// <summary>
    /// Gets or sets the subkey of the group policy to delete.
    /// </summary>
    public string SubKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the value name of the group policy to delete.
    /// </summary>
    public string ValueName { get; set; } = string.Empty;
}
