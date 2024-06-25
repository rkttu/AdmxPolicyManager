using AdmxPolicyManager.Models.Policies;
using System;
using System.Collections.Generic;
using System.Linq;

// Please do not update the namespace for convience of usage in the consumer projects.
namespace AdmxPolicyManager
{
    /// <summary>
    /// Provides extension methods for PolicyRegistry.
    /// </summary>
    public static class PolicyRegistryExtensions
    {
        /// <summary>
        /// Queries the status of a policy for a specific user or group.
        /// </summary>
        /// <param name="policy">The policy to query.</param>
        /// <param name="userOrGroupSid">The SID of the user or group.</param>
        /// <returns>The status of the policy.</returns>
        public static PolicyStatus QueryPolicyStatus(this PolicyInfoBase policy, string userOrGroupSid)
        {
            var section = policy is MachinePolicyInfo ? PolicySection.Machine : PolicySection.User;
            var allEnabledListConfigured = policy.EnabledList.Count() > 0 && policy.EnabledList.All(x => x.IsApplied(section, userOrGroupSid) == PolicyValueStatus.ConfiguredAndMatches);
            var allDisabledListConfigured = policy.DisabledList.Count() > 0 && policy.DisabledList.All(x => x.IsApplied(section, userOrGroupSid) == PolicyValueStatus.ConfiguredAndMatches);

            if (allEnabledListConfigured && !allDisabledListConfigured)
                return PolicyStatus.ConfiguredAndEnabled;
            if (!allEnabledListConfigured && allDisabledListConfigured)
                return PolicyStatus.ConfiguredAndDisabled;
            return PolicyStatus.NotConfigured;
        }

        /// <summary>
        /// Enables a policy for a specific subject.
        /// </summary>
        /// <param name="policy">The policy to enable.</param>
        /// <param name="subjectId">The ID of the subject.</param>
        /// <param name="userOrGroupSid">The SID of the user or group.</param>
        /// <param name="retryCount">The number of retries for saving the policy.</param>
        /// <returns>A list of modified registries and their modify results.</returns>
        public static IReadOnlyList<Tuple<PolicyRegistryValue, PolicyModifyResult>> EnablePolicy(this PolicyInfoBase policy, Guid subjectId, string userOrGroupSid, int retryCount = GroupPolicy.DefaultSaveRetryCount)
        {
            var section = policy is MachinePolicyInfo ? PolicySection.Machine : PolicySection.User;
            var modifiedRegistries = new List<Tuple<PolicyRegistryValue, PolicyModifyResult>>();
            modifiedRegistries.AddRange(policy.ResetPolicy(subjectId, userOrGroupSid, retryCount));
            if (policy.EnabledList.Count > 0)
                modifiedRegistries.AddRange(policy.EnabledList.Select(x => Tuple.Create(x, x.Apply(section, subjectId, userOrGroupSid, retryCount))));
            if (policy.DefaultValueList.Count > 0)
                modifiedRegistries.AddRange(policy.DefaultValueList.Select(x => Tuple.Create(x, x.Apply(section, subjectId, userOrGroupSid, retryCount))));
            return modifiedRegistries.AsReadOnly();
        }

        /// <summary>
        /// Disables a policy for a specific subject.
        /// </summary>
        /// <param name="policy">The policy to disable.</param>
        /// <param name="subjectId">The ID of the subject.</param>
        /// <param name="userOrGroupSid">The SID of the user or group.</param>
        /// <param name="retryCount">The number of retries for saving the policy.</param>
        /// <returns>A list of modified registries and their modify results.</returns>
        public static IReadOnlyList<Tuple<PolicyRegistryValue, PolicyModifyResult>> DisablePolicy(this PolicyInfoBase policy, Guid subjectId, string userOrGroupSid, int retryCount = GroupPolicy.DefaultSaveRetryCount)
        {
            var section = policy is MachinePolicyInfo ? PolicySection.Machine : PolicySection.User;
            var modifiedRegistries = new List<Tuple<PolicyRegistryValue, PolicyModifyResult>>();
            modifiedRegistries.AddRange(policy.ResetPolicy(subjectId, userOrGroupSid, retryCount));
            if (policy.DisabledList.Count > 0)
                modifiedRegistries.AddRange(policy.DisabledList.Select(x => Tuple.Create(x, x.Apply(section, subjectId, userOrGroupSid, retryCount))));
            return modifiedRegistries.AsReadOnly();
        }

        /// <summary>
        /// Resets a policy for a specific subject.
        /// </summary>
        /// <param name="policy">The policy to reset.</param>
        /// <param name="subjectId">The ID of the subject.</param>
        /// <param name="userOrGroupSid">The SID of the user or group.</param>
        /// <param name="retryCount">The number of retries for saving the policy.</param>
        /// <returns>A list of modified registries and their modify results.</returns>
        public static IReadOnlyList<Tuple<PolicyRegistryValue, PolicyModifyResult>> ResetPolicy(this PolicyInfoBase policy, Guid subjectId, string userOrGroupSid, int retryCount = GroupPolicy.DefaultSaveRetryCount)
        {
            var section = policy is MachinePolicyInfo ? PolicySection.Machine : PolicySection.User;
            var modifiedRegistries = new List<Tuple<PolicyRegistryValue, PolicyModifyResult>>();
            if (policy.EnabledList.Count > 0)
                modifiedRegistries.AddRange(policy.EnabledList.Select(x => Tuple.Create(x, x.Reset(section, subjectId, userOrGroupSid, retryCount))));
            if (policy.DefaultValueList.Count > 0)
                modifiedRegistries.AddRange(policy.DefaultValueList.Select(x => Tuple.Create(x, x.Reset(section, subjectId, userOrGroupSid, retryCount))));
            if (policy.DisabledList.Count > 0)
                modifiedRegistries.AddRange(policy.DisabledList.Select(x => Tuple.Create(x, x.Reset(section, subjectId, userOrGroupSid, retryCount))));
            return modifiedRegistries.AsReadOnly();
        }
    }
}
