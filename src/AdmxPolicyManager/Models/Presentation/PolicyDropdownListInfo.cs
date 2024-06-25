using AdmxPolicyManager.Contracts.Presentation;

namespace AdmxPolicyManager.Models.Presentation
{
    /// <summary>
    /// Represents information about a policy dropdown list control in the presentation layer.
    /// </summary>
    public sealed class PolicyDropdownListInfo : IPresentationControlHasRefId
    {
        internal PolicyDropdownListInfo() { }

        /// <summary>
        /// Gets or sets the reference ID of the policy dropdown list control.
        /// </summary>
        public string RefId { get; internal set; } = default;

        /// <summary>
        /// Gets or sets a value indicating whether the dropdown list should be sorted.
        /// </summary>
        public bool NoSort { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the index of the default item in the dropdown list.
        /// </summary>
        public uint DefaultItemIndex { get; internal set; } = default;

        /// <summary>
        /// Gets or sets a value indicating whether the default item is specified.
        /// </summary>
        public bool DefaultItemSpecified { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the label of the policy dropdown list control.
        /// </summary>
        public string Label { get; internal set; } = default;
    }
}
