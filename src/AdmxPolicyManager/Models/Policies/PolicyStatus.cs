namespace AdmxPolicyManager.Models.Policies
{
    /// <summary>
    /// Represents the status of a policy.
    /// </summary>
    public enum PolicyStatus
    {
        /// <summary>
        /// The policy is not configured.
        /// </summary>
        NotConfigured,

        /// <summary>
        /// The policy is configured and enabled.
        /// </summary>
        ConfiguredAndEnabled,

        /// <summary>
        /// The policy is configured and disabled.
        /// </summary>
        ConfiguredAndDisabled,
    }
}