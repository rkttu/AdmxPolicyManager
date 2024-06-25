using AdmxPolicyManager.Contracts.Presentation;

namespace AdmxPolicyManager.Models.Presentation
{
    /// <summary>
    /// Represents the information of a policy list box control in the presentation.
    /// </summary>
    public sealed class PolicyListBoxInfo : IPresentationControlHasRefId
    {
        internal PolicyListBoxInfo() { }

        /// <summary>
        /// Gets or sets the reference ID of the policy list box control.
        /// </summary>
        public string RefId { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the label of the policy list box control.
        /// </summary>
        public string Label { get; internal set; } = default;
    }
}
