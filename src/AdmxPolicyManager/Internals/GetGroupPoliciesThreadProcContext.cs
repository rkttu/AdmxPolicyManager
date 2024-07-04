namespace AdmxPolicyManager.Internals;

internal sealed class GetGroupPoliciesThreadProcContext
{
    public GroupPolicyLocation Location { get; set; }
    public string LocationParameter { get; set; } = string.Empty;
    public bool IsMachine { get; set; }
    public string SubKey { get; set; } = string.Empty;
    public string? ValueNamePrefix { get; set; }
    public bool UseCriticalPolicySection { get; set; } = false;
    public MultipleGroupPolicyQueryResult Result { get; set; } = new MultipleGroupPolicyQueryResult();
}
