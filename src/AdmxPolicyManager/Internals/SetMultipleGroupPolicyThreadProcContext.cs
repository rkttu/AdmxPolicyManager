using AdmxPolicyManager.Models;
using System.Collections.Generic;
using System.Linq;

namespace AdmxPolicyManager.Internals;

internal sealed class SetMultipleGroupPolicyThreadProcContext
{
    public GroupPolicyLocation Location { get; set; }
    public string LocationParameter { get; set; } = string.Empty;
    public bool IsMachine { get; set; }
    public IEnumerable<SetMultipleGroupPolicyRequest> Requests { get; set; } = Enumerable.Empty<SetMultipleGroupPolicyRequest>();
    public int RetryCount { get; set; } = GroupPolicy.DefaultRetryCount;
}
