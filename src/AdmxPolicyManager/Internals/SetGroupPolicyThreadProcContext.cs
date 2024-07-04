namespace AdmxPolicyManager.Internals;

internal sealed class SetGroupPolicyThreadProcContext
{
    public GroupPolicyLocation Location { get; set; }
    public string LocationParameter { get; set; } = string.Empty;
    public bool IsMachine { get; set; }
    public string SubKey { get; set; } = string.Empty;
    public string ValueName { get; set; } = string.Empty;
    public object Value { get; set; } = default!;
    public bool RequireExpandString { get; set; } = default;
    public int RetryCount { get; set; } = GroupPolicy.DefaultRetryCount;
    public GroupPolicyUpdateResult Result { get; set; } = GroupPolicyUpdateResult.UpdateSucceed;
}
