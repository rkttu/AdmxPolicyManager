using AdmxPolicyManager.Contracts.Policies;
using AdmxPolicyManager.Models.Policies;
using AdmxPolicyManager.Models.Presentation;

namespace AdmxPolicyManager.Models.Elements
{
    /// <summary>
    /// Represents the information of a multi-text policy element.
    /// </summary>
    public sealed class PolicyMultiTextElementInfo : IElementInfo<PolicyMultiTextBoxInfo>
    {
        internal PolicyMultiTextElementInfo() { }

        /// <summary>
        /// Gets or sets the presentation information of the policy element.
        /// </summary>
        public PolicyMultiTextBoxInfo Presentation { get; internal set; } = default;

        /// <summary>
        /// Gets the type of the policy element.
        /// </summary>
        public PolicyElementType Type => PolicyElementType.MultiText;

        /// <summary>
        /// Gets or sets the client extension of the policy element.
        /// </summary>
        public string ClientExtension { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the ID of the policy element.
        /// </summary>
        public string Id { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the maximum length of the multi-text value.
        /// </summary>
        public uint MaxLength { get; internal set; }

        /// <summary>
        /// Gets or sets the maximum number of strings in the multi-text value.
        /// </summary>
        public uint MaxStrings { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether the policy element is required.
        /// </summary>
        public bool Required { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether the policy element is soft.
        /// </summary>
        public bool Soft { get; internal set; }

        /// <summary>
        /// Gets or sets the registry value of the policy element.
        /// </summary>
        public PolicyRegistryValue RegistryValue { get; internal set; } = default;
    }
}
