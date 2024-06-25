using AdmxPolicyManager.Contracts.Policies;
using AdmxPolicyManager.Models.Policies;
using AdmxPolicyManager.Models.Presentation;

namespace AdmxPolicyManager.Models.Elements
{
    /// <summary>
    /// Represents the information of a decimal policy element.
    /// </summary>
    public sealed class PolicyDecimalElementInfo : IElementInfo<PolicyDecimalTextBoxInfo>
    {
        internal PolicyDecimalElementInfo() { }

        /// <summary>
        /// Gets or sets the presentation information of the decimal policy element.
        /// </summary>
        public PolicyDecimalTextBoxInfo Presentation { get; internal set; } = default;

        /// <summary>
        /// Gets the type of the policy element.
        /// </summary>
        public PolicyElementType Type => PolicyElementType.Decimal;

        /// <summary>
        /// Gets or sets the client extension of the decimal policy element.
        /// </summary>
        public string ClientExtension { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the ID of the decimal policy element.
        /// </summary>
        public string Id { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the maximum value of the decimal policy element.
        /// </summary>
        public uint MaxValue { get; internal set; }

        /// <summary>
        /// Gets or sets the minimum value of the decimal policy element.
        /// </summary>
        public uint MinValue { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether the decimal policy element is required.
        /// </summary>
        public bool Required { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether the decimal policy element is soft.
        /// </summary>
        public bool Soft { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether the decimal policy element should be stored as text.
        /// </summary>
        public bool StoreAsText { get; internal set; }

        /// <summary>
        /// Gets or sets the default value of the decimal policy element.
        /// </summary>
        public uint? DefaultValue { get; internal set; }

        /// <summary>
        /// Gets or sets the registry value of the decimal policy element.
        /// </summary>
        public PolicyRegistryValue RegistryValue { get; internal set; } = default;
    }
}
