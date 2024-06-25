namespace AdmxPolicyManager.Models.Policies
{
    /// <summary>
    /// Represents the result of modifying a policy.
    /// </summary>
    public enum PolicyModifyResult
    {
        /// <summary>
        /// The policy was not changed.
        /// </summary>
        NotChanged,

        /// <summary>
        /// The policy was updated.
        /// </summary>
        Updated,

        /// <summary>
        /// The policy was deleted.
        /// </summary>
        Deleted,

        /// <summary>
        /// The policy was skipped.
        /// </summary>
        Skipped,
    }
}