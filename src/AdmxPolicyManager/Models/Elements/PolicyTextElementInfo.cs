using AdmxPolicyManager.Contracts.Policies;
using AdmxPolicyManager.Contracts.Presentation;
using AdmxPolicyManager.Models.Policies;
using AdmxPolicyManager.Models.Presentation;

namespace AdmxPolicyManager.Models.Elements
{
    /// <summary>
    /// Represents the information of a policy text element.
    /// </summary>
    public sealed class PolicyTextElementInfo : IElementInfo<IPresentationControlHasRefId>
    {
        internal PolicyTextElementInfo() { }

        /// <summary>
        /// Gets or sets the presentation control of the policy text element.
        /// </summary>
        public IPresentationControlHasRefId Presentation { get; internal set; } = default;

        /// <summary>
        /// Gets the type of the policy element.
        /// </summary>
        public PolicyElementType Type => PolicyElementType.Text;

        /// <summary>
        /// Gets or sets the client extension of the policy text element.
        /// </summary>
        public string ClientExtension { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the ID of the policy text element.
        /// </summary>
        public string Id { get; internal set; } = default;

        /// <summary>
        /// Gets or sets a value indicating whether the policy text element is expandable.
        /// </summary>
        public bool Expandable { get; internal set; }

        /// <summary>
        /// Gets or sets the maximum length of the policy text element.
        /// </summary>
        public uint MaxLength { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether the policy text element is required.
        /// </summary>
        public bool Required { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether the policy text element is soft.
        /// </summary>
        public bool Soft { get; internal set; }

        /// <summary>
        /// Gets or sets the default value of the policy text element.
        /// </summary>
        public string DefaultValue { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the registry value of the policy text element.
        /// </summary>
        public PolicyRegistryValue RegistryValue { get; internal set; } = default;

        /// <summary>
        /// Gets the presentation control of the policy text element as a <see cref="PolicyTextBoxInfo"/>.
        /// </summary>
        public PolicyTextBoxInfo TextBoxPresentation => Presentation as PolicyTextBoxInfo;

        /// <summary>
        /// Gets the presentation control of the policy text element as a <see cref="PolicyComboBoxInfo"/>.
        /// </summary>
        public PolicyComboBoxInfo ComboBoxPresentation => Presentation as PolicyComboBoxInfo;

        PolicyRegistryValue IElementInfo.RegistryValue => throw new System.NotImplementedException();
    }
}
