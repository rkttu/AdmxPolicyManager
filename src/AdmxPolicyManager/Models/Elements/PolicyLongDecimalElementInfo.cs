using AdmxPolicyManager.Contracts.Policies;
using AdmxPolicyManager.Models.Policies;
using AdmxPolicyManager.Models.Presentation;

namespace AdmxPolicyManager.Models.Elements
{
    /// <summary>
    /// Represents the information of a policy element with a long decimal value.
    /// </summary>
    public sealed class PolicyLongDecimalElementInfo : IElementInfo<PolicyLongDecimalTextBoxInfo>
    {
        internal PolicyLongDecimalElementInfo() { }

        /// <summary>
        /// Gets or sets the presentation information of the policy element.
        /// </summary>
        public PolicyLongDecimalTextBoxInfo Presentation { get; internal set; } = default;

        /// <summary>
        /// Gets the type of the policy element.
        /// </summary>
        public PolicyElementType Type => PolicyElementType.LongDecimal;

        /// <summary>
        /// Gets or sets the client extension of the policy element.
        /// </summary>
        public string ClientExtension { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the ID of the policy element.
        /// </summary>
        public string Id { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the maximum value of the policy element.
        /// </summary>
        public ulong MaxValue { get; internal set; }

        /// <summary>
        /// Gets or sets the minimum value of the policy element.
        /// </summary>
        public ulong MinValue { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether the policy element is required.
        /// </summary>
        public bool Required { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether the policy element is soft.
        /// </summary>
        public bool Soft { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether the policy element should be stored as text.
        /// </summary>
        public bool StoreAsText { get; internal set; }

        /// <summary>
        /// Gets or sets the default value of the policy element.
        /// </summary>
        public ulong? DefaultValue { get; internal set; }

        /// <summary>
        /// Gets or sets the registry value of the policy element.
        /// </summary>
        public PolicyRegistryValue RegistryValue { get; internal set; } = default;
    }
}
