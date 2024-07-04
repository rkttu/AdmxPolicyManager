using System.Collections.Generic;

namespace AdmxPolicyManager;

/// <summary>
/// Represents the result of a multiple group policy query.
/// </summary>
public sealed class MultipleGroupPolicyQueryResult
{
    /// <summary>
    /// Gets or sets the last error code.
    /// </summary>
    public int LastErrorCode { get; internal set; } = NativeMethods.S_OK;

    /// <summary>
    /// Gets or sets a value indicating whether the query succeeded.
    /// </summary>
    public bool Succeed { get; internal set; } = false;

    /// <summary>
    /// Gets or sets the value prefix.
    /// </summary>
    public string ValuePrefix { get; internal set; } = string.Empty;

    /// <summary>
    /// Gets the list of group policy query results.
    /// </summary>
    public List<GroupPolicyQueryResult> Results { get; } = new List<GroupPolicyQueryResult>();
}
