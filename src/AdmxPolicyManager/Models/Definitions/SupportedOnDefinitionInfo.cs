using AdmxPolicyManager.Contracts.Presentation;
using AdmxPolicyManager.Models.Resources;
using System;
using System.Collections.Generic;

namespace AdmxPolicyManager.Models.Definitions
{
    /// <summary>
    /// Represents information about the supported conditions for a policy definition.
    /// </summary>
    public sealed class SupportedOnDefinitionInfo : IResolvable
    {
        internal SupportedOnDefinitionInfo() { }

        /// <summary>
        /// Gets or sets the base display name of the supported conditions.
        /// </summary>
        public string BaseDisplayName { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the name of the definition.
        /// </summary>
        public string DefinitionName { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the display name key for the supported conditions.
        /// </summary>
        public ResourceKeyReference DisplayNameKey { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the list of supported AND conditions.
        /// </summary>
        public IReadOnlyList<SupportedCriteriaInfo> AndConditions { get; internal set; } = Array.Empty<SupportedCriteriaInfo>();

        /// <summary>
        /// Gets or sets the list of supported OR conditions.
        /// </summary>
        public IReadOnlyList<SupportedCriteriaInfo> OrConditions { get; internal set; } = Array.Empty<SupportedCriteriaInfo>();

        /// <summary>
        /// Gets a value indicating whether the supported conditions have been resolved.
        /// </summary>
        public bool Resolved { get; private set; }

        /// <summary>
        /// Resolves the supported conditions using the specified catalog and policy definition information.
        /// </summary>
        /// <param name="catalog">The policy definition catalog.</param>
        /// <param name="policyDefinitionInfo">The policy definition information.</param>
        public void Resolve(PolicyDefinitionCatalogInfo catalog, PolicyDefinitionInfo policyDefinitionInfo)
        {
            if (Resolved)
                return;

            if (policyDefinitionInfo.TryGetFallbackResource(out var foundResource) && foundResource != null)
            {
                if (DisplayNameKey.TryResolveStringFromFallbackCulture(policyDefinitionInfo, out var productDisplayName) && productDisplayName != null)
                    BaseDisplayName = productDisplayName;
            }

            Resolved = true;
        }
    }
}
