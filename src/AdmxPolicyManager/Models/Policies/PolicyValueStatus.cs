namespace AdmxPolicyManager.Models.Policies
{
    /// <summary>
    /// Represents the status of a policy value.
    /// </summary>
    public enum PolicyValueStatus
    {
        /// <summary>
        /// The policy value is not configured.
        /// </summary>
        NotConfigured,

        /// <summary>
        /// The policy value is configured and matches the desired value.
        /// </summary>
        ConfiguredAndMatches,

        /// <summary>
        /// The policy value is configured but differs from the desired value.
        /// </summary>
        ConfiguredButDifferent,
    }
}