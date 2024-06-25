using AdmxParser.Serialization;
using AdmxPolicyManager.Contracts.Presentation;
using AdmxPolicyManager.Models.Definitions;
using AdmxPolicyManager.Models.Policies;
using AdmxPolicyManager.Models.Resources;
using System;
using System.Collections.Generic;

namespace AdmxPolicyManager.Models.Elements
{
    /// <summary>
    /// Represents an item of an enumeration element.
    /// </summary>
    public sealed class EnumerationElementItemInfo : IResolvable
    {
        internal EnumerationElementItemInfo() { }

        /// <summary>
        /// Gets or sets the base display name of the item.
        /// </summary>
        public string BaseDisplayName { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the ID of the item.
        /// </summary>
        public string Id { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the display name key of the item.
        /// </summary>
        public ResourceKeyReference DisplayNameKey { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the value of the item.
        /// </summary>
        public Value Value { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the list of policy registry values associated with the item.
        /// </summary>
        public IReadOnlyList<PolicyRegistryValue> ItemValues { get; internal set; } = Array.Empty<PolicyRegistryValue>();

        /// <summary>
        /// Gets a value indicating whether the item has been resolved.
        /// </summary>
        public bool Resolved { get; private set; }

        /// <summary>
        /// Resolves the item using the specified policy definition catalog and policy definition information.
        /// </summary>
        /// <param name="catalog">The policy definition catalog.</param>
        /// <param name="policyDefinitionInfo">The policy definition information.</param>
        public void Resolve(PolicyDefinitionCatalogInfo catalog, PolicyDefinitionInfo policyDefinitionInfo)
        {
            if (Resolved)
                return;

            if (policyDefinitionInfo.TryGetFallbackResource(out var foundResource) && foundResource != null)
            {
                if (DisplayNameKey.TryResolveStringFromFallbackCulture(policyDefinitionInfo, out var foundDisplayName) && foundDisplayName != null)
                    BaseDisplayName = foundDisplayName;
            }

            Resolved = true;
        }

        /// <summary>
        /// Returns a string representation of the item.
        /// </summary>
        /// <returns>A string representation of the item.</returns>
        public override string ToString()
            => Resolved ? $"{BaseDisplayName} ({Id})" : $"{DisplayNameKey} ({Id})";
    }
}
