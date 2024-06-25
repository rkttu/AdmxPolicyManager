using AdmxPolicyManager.Contracts.Presentation;

namespace AdmxPolicyManager.Models.Presentation
{
    /// <summary>
    /// Represents the information for a policy long decimal text box control in the presentation layer.
    /// </summary>
    public sealed class PolicyLongDecimalTextBoxInfo : IPresentationControlHasRefId
    {
        internal PolicyLongDecimalTextBoxInfo() { }

        /// <summary>
        /// Gets or sets the reference ID of the control.
        /// </summary>
        public string RefId { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the default value of the control.
        /// </summary>
        public ulong Default { get; internal set; } = 1uL;

        /// <summary>
        /// Gets or sets a value indicating whether the control has spin buttons.
        /// </summary>
        public bool Spin { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the step value for the spin buttons.
        /// </summary>
        public uint SpinStep { get; internal set; } = 1u;

        /// <summary>
        /// Gets or sets the label of the control.
        /// </summary>
        public string Label { get; internal set; } = default;
    }
}
