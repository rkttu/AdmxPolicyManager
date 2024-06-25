namespace AdmxPolicyManager.Models.Policies
{
    /// <summary>
    /// Represents the machine policy information.
    /// </summary>
    public sealed class MachinePolicyInfo : PolicyInfoBase
    {
        /// <summary>
        /// Gets the target section of the policy.
        /// </summary>
        public override PolicySection TargetSection => PolicySection.Machine;
    }
}
