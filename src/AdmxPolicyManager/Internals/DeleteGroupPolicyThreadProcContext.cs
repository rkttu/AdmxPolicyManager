using System.Collections;

namespace AdmxPolicyManager.Internals;

internal sealed class DeleteGroupPolicyThreadProcContext
{
    public GroupPolicyLocation Location { get; set; }
    public string LocationParameter { get; set; } = string.Empty;
    public bool IsMachine { get; set; }
    public string SubKey { get; set; } = string.Empty;
    public string ValueName { get; set; } = string.Empty;
    public int RetryCount { get; set; } = GroupPolicy.DefaultRetryCount;
    public GroupPolicyDeleteResult Result { get; set; } = GroupPolicyDeleteResult.DeleteSucceed;
}
