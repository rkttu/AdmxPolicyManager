using AdmxParser.Serialization;
using AdmxPolicyManager.Contracts.Policies;
using AdmxPolicyManager.Models.Policies;
using AdmxPolicyManager.Models.Presentation;
using System;
using System.Collections.Generic;

namespace AdmxPolicyManager.Models.Elements
{
    /// <summary>
    /// Represents the information of a boolean policy element.
    /// </summary>
    public sealed class PolicyBooleanElementInfo : IElementInfo<PolicyCheckBoxInfo>
    {
        internal PolicyBooleanElementInfo() { }

        /// <summary>
        /// Gets or sets the presentation information of the policy element.
        /// </summary>
        public PolicyCheckBoxInfo Presentation { get; internal set; } = default;

        /// <summary>
        /// Gets the type of the policy element.
        /// </summary>
        public PolicyElementType Type => PolicyElementType.Boolean;

        /// <summary>
        /// Gets or sets the client extension of the policy element.
        /// </summary>
        public string ClientExtension { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the ID of the policy element.
        /// </summary>
        public string Id { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the true value of the policy element.
        /// </summary>
        public Value TrueValue { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the false value of the policy element.
        /// </summary>
        public Value FalseValue { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the list of true values for the policy element.
        /// </summary>
        public IReadOnlyList<PolicyRegistryValue> TrueValueList { get; internal set; } = Array.Empty<PolicyRegistryValue>();

        /// <summary>
        /// Gets or sets the list of false values for the policy element.
        /// </summary>
        public IReadOnlyList<PolicyRegistryValue> FalseValueList { get; internal set; } = Array.Empty<PolicyRegistryValue>();

        /// <summary>
        /// Gets or sets the default value of the policy element.
        /// </summary>
        public bool? DefaultValue { get; internal set; }

        /// <summary>
        /// Gets or sets the registry value of the policy element.
        /// </summary>
        public PolicyRegistryValue RegistryValue { get; internal set; } = default;
    }
}
