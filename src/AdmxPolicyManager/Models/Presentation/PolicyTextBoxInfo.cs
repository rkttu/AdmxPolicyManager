using AdmxPolicyManager.Contracts.Presentation;

namespace AdmxPolicyManager.Models.Presentation
{
    /// <summary>
    /// Represents the information of a policy text box control.
    /// </summary>
    public sealed class PolicyTextBoxInfo : IPresentationControlHasRefId
    {
        internal PolicyTextBoxInfo() { }

        /// <summary>
        /// Gets or sets the reference ID of the policy text box control.
        /// </summary>
        public string RefId { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the label of the policy text box control.
        /// </summary>
        public string Label { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the default value of the policy text box control.
        /// </summary>
        public string Default { get; internal set; } = default;
    }
}
