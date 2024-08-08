namespace AdmxPolicyManager.Models;

/// <summary>
/// Request model for setting multiple group policies.
/// </summary>
public sealed class SetMultipleGroupPolicyRequest
{
    /// <summary>
    /// Gets or sets the subkey of the group policy to set.
    /// </summary>
    public string SubKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the value name of the group policy to set.
    /// </summary>
    public string ValueName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the value of the group policy to set.
    /// </summary>
    public object Value { get; set; } = null!;

    /// <summary>
    /// Gets or sets a value indicating whether the value should expand envrionment variable.
    /// </summary>
    public bool RequireExpandString { get; set; }
}
