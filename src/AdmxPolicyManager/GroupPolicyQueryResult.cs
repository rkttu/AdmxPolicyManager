namespace AdmxPolicyManager;

/// <summary>
/// Contains the result of a group policy query.
/// </summary>
public sealed class GroupPolicyQueryResult
{
    /// <summary>
    /// Gets or sets the key path.
    /// </summary>
    public string KeyPath { get; internal set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the key exists.
    /// </summary>
    public bool KeyExists { get; internal set; } = false;

    /// <summary>
    /// Gets or sets the value name.
    /// </summary>
    public string ValueName { get; internal set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the value exists.
    /// </summary>
    public bool ValueExists { get; internal set; } = false;

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public object? Value { get; internal set; } = null;

    /// <summary>
    /// Gets or sets the value type.
    /// </summary>
    public RegistryValueType ValueType { get; internal set; } = RegistryValueType.None;
}
