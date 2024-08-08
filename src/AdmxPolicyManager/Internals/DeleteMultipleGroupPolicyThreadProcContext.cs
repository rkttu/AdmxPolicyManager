using AdmxPolicyManager.Models;
using System.Collections.Generic;
using System.Linq;

namespace AdmxPolicyManager.Internals;

internal sealed class DeleteMultipleGroupPolicyThreadProcContext
{
    public GroupPolicyLocation Location { get; set; }
    public string LocationParameter { get; set; } = string.Empty;
    public bool IsMachine { get; set; }
    public IEnumerable<DeleteMultipleGroupPolicyRequest> Requests { get; set; } = Enumerable.Empty<DeleteMultipleGroupPolicyRequest>();
    public int RetryCount { get; set; } = GroupPolicy.DefaultRetryCount;
    public IEnumerable<GroupPolicyDeleteResult> Result { get; set; } = Enumerable.Empty<GroupPolicyDeleteResult>();
}
