using AdmxPolicyManager.Contracts.Presentation;

namespace AdmxPolicyManager.Models.Presentation
{
    /// <summary>
    /// Represents the information of a multi-textbox policy control in the presentation layer.
    /// </summary>
    public sealed class PolicyMultiTextBoxInfo : IPresentationControlHasRefId
    {
        internal PolicyMultiTextBoxInfo() { }

        /// <summary>
        /// Gets or sets the reference ID of the multi-textbox policy control.
        /// </summary>
        public string RefId { get; internal set; } = default;

        /// <summary>
        /// Gets or sets a value indicating whether the multi-textbox policy control should be shown as a dialog.
        /// </summary>
        public bool ShowAsDialog { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the default height of the multi-textbox policy control.
        /// </summary>
        public uint DefaultHeight { get; internal set; } = 3u;
    }
}
