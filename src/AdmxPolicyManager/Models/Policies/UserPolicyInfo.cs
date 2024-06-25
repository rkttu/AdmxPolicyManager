namespace AdmxPolicyManager.Models.Policies
{
    /// <summary>
    /// Represents information about a user policy.
    /// </summary>
    public sealed class UserPolicyInfo : PolicyInfoBase
    {
        /// <summary>
        /// Gets the target section of the policy, which is the User section.
        /// </summary>
        public override PolicySection TargetSection => PolicySection.User;
    }
}
