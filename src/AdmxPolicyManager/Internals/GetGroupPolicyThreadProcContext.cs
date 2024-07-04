namespace AdmxPolicyManager.Internals;

internal sealed class GetGroupPolicyThreadProcContext
{
    public GroupPolicyLocation Location { get; set; }
    public string LocationParameter { get; set; } = string.Empty;
    public bool IsMachine { get; set; }
    public string SubKey { get; set; } = string.Empty;
    public string ValueName { get; set; } = string.Empty;
    public bool UseCriticalPolicySection { get; set; } = false;
    public GroupPolicyQueryResult Result { get; set; } = new GroupPolicyQueryResult();
}
