using AdmxPolicyManager.Contracts.Policies;
using AdmxPolicyManager.Contracts.Presentation;
using AdmxPolicyManager.Models.Definitions;
using AdmxPolicyManager.Models.Policies;
using AdmxPolicyManager.Models.Presentation;
using System;
using System.Collections.Generic;

namespace AdmxPolicyManager.Models.Elements
{
    /// <summary>
    /// Represents an enumeration policy element information.
    /// </summary>
    public sealed class PolicyEnumerationElementInfo : IElementInfo<PolicyDropdownListInfo>, IResolvable
    {
        internal PolicyEnumerationElementInfo() { }

        /// <summary>
        /// Gets or sets the presentation information for the policy element.
        /// </summary>
        public PolicyDropdownListInfo Presentation { get; internal set; } = default;

        /// <summary>
        /// Gets the type of the policy element.
        /// </summary>
        public PolicyElementType Type => PolicyElementType.Enumeration;

        /// <summary>
        /// Gets or sets the client extension for the policy element.
        /// </summary>
        public string ClientExtension { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the ID of the policy element.
        /// </summary>
        public string Id { get; internal set; } = default;

        /// <summary>
        /// Gets or sets a value indicating whether the policy element is required.
        /// </summary>
        public bool Required { get; internal set; }

        /// <summary>
        /// Gets or sets the list of items for the enumeration policy element.
        /// </summary>
        public IReadOnlyList<EnumerationElementItemInfo> Items { get; internal set; } = Array.Empty<EnumerationElementItemInfo>();

        /// <summary>
        /// Gets or sets the default value for the enumeration policy element.
        /// </summary>
        public int? DefaultValue { get; internal set; }

        /// <summary>
        /// Gets or sets the registry value for the enumeration policy element.
        /// </summary>
        public PolicyRegistryValue RegistryValue { get; internal set; } = default;

        /// <summary>
        /// Gets a value indicating whether the policy element has been resolved.
        /// </summary>
        public bool Resolved { get; private set; }

        /// <summary>
        /// Resolves the policy element using the specified catalog and policy definition information.
        /// </summary>
        /// <param name="catalog">The policy definition catalog.</param>
        /// <param name="policyDefinitionInfo">The policy definition information.</param>
        public void Resolve(PolicyDefinitionCatalogInfo catalog, PolicyDefinitionInfo policyDefinitionInfo)
        {
            if (Resolved)
                return;

            foreach (var eachItem in Items)
                eachItem.Resolve(catalog, policyDefinitionInfo);

            Resolved = true;
        }
    }
}
