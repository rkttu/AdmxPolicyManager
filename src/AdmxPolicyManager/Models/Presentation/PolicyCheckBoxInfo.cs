using AdmxPolicyManager.Contracts.Presentation;

namespace AdmxPolicyManager.Models.Presentation
{
    /// <summary>
    /// Represents a policy checkbox control in the presentation layer.
    /// </summary>
    public sealed class PolicyCheckBoxInfo : IPresentationControlHasRefId
    {
        internal PolicyCheckBoxInfo() { }

        /// <summary>
        /// Gets or sets the reference ID of the policy checkbox.
        /// </summary>
        public string RefId { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the default value of the policy checkbox.
        /// </summary>
        public bool Default { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the label of the policy checkbox.
        /// </summary>
        public string Label { get; internal set; } = default;
    }
}
