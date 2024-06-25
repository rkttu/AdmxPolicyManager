namespace AdmxPolicyManager.Models.Policies
{
    /// <summary>
    /// Represents the different sections of a policy.
    /// </summary>
    public enum PolicySection : int
    {
        /// <summary>
        /// The root section of the policy.
        /// </summary>
        Root = 0,

        /// <summary>
        /// The user section of the policy.
        /// </summary>
        User = 1,

        /// <summary>
        /// The machine section of the policy.
        /// </summary>
        Machine = 2,
    }
}
