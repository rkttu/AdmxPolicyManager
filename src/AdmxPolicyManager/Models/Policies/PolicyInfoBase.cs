using AdmxPolicyManager.Contracts.Policies;
using AdmxPolicyManager.Contracts.Presentation;
using AdmxPolicyManager.Models.Definitions;
using AdmxPolicyManager.Models.Elements;
using AdmxPolicyManager.Models.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AdmxPolicyManager.Models.Policies
{
    /// <summary>
    /// Represents the base class for policy information.
    /// </summary>
    public abstract class PolicyInfoBase : IResolvable
    {
        internal PolicyInfoBase() { }

        /// <summary>
        /// Gets or sets the base display name of the policy.
        /// </summary>
        public string BaseDisplayName { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the base explain text of the policy.
        /// </summary>
        public string BaseExplainText { get; internal set; } = default;

        /// <summary>
        /// Gets the base category display names of the policy.
        /// </summary>
        public IReadOnlyList<string> BaseCategoryDisplayNames { get; internal set; } = Array.Empty<string>();

        /// <summary>
        /// Gets or sets the name of the policy.
        /// </summary>
        public string Name { get; internal set; } = default;

        /// <summary>
        /// Gets the target section of the policy.
        /// </summary>
        public abstract PolicySection TargetSection { get; }

        /// <summary>
        /// Gets or sets the display name key of the policy.
        /// </summary>
        public ResourceKeyReference DisplayNameKey { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the explain text key of the policy.
        /// </summary>
        public ResourceKeyReference ExplainTextKey { get; internal set; } = default;

        /// <summary>
        /// Gets the category keys of the policy.
        /// </summary>
        public IReadOnlyList<EntityReference> CategoryKeys { get; internal set; } = Array.Empty<EntityReference>();

        /// <summary>
        /// Gets or sets the keywords of the policy.
        /// </summary>
        public string Keywords { get; internal set; } = default;

        /// <summary>
        /// Gets the see also references of the policy.
        /// </summary>
        public IReadOnlyList<string> SeeAlso { get; internal set; } = Array.Empty<string>();

        /// <summary>
        /// Gets or sets the supported on key of the policy.
        /// </summary>
        public EntityReference SupportedOnKey { get; internal set; } = default;

        /// <summary>
        /// Gets the superseded files of the policy.
        /// </summary>
        public IReadOnlyList<string> SupersededFiles { get; internal set; } = Array.Empty<string>();

        /// <summary>
        /// Gets the enabled list of the policy.
        /// </summary>
        public IReadOnlyList<PolicyRegistryValue> EnabledList { get; internal set; } = Array.Empty<PolicyRegistryValue>();

        /// <summary>
        /// Gets the disabled list of the policy.
        /// </summary>
        public IReadOnlyList<PolicyRegistryValue> DisabledList { get; internal set; } = Array.Empty<PolicyRegistryValue>();

        /// <summary>
        /// Gets the default value list of the policy.
        /// </summary>
        public IReadOnlyList<PolicyRegistryValue> DefaultValueList { get; internal set; } = Array.Empty<PolicyRegistryValue>();

        /// <summary>
        /// Gets the elements of the policy.
        /// </summary>
        public IReadOnlyList<IElementInfo> Elements { get; internal set; } = Array.Empty<IElementInfo>();

        /// <summary>
        /// Gets the boolean elements of the policy.
        /// </summary>
        public IReadOnlyList<PolicyBooleanElementInfo> BooleanElements { get; internal set; } = Array.Empty<PolicyBooleanElementInfo>();

        /// <summary>
        /// Gets the decimal elements of the policy.
        /// </summary>
        public IReadOnlyList<PolicyDecimalElementInfo> DecimalElements { get; internal set; } = Array.Empty<PolicyDecimalElementInfo>();

        /// <summary>
        /// Gets the enumeration elements of the policy.
        /// </summary>
        public IReadOnlyList<PolicyEnumerationElementInfo> EnumerationElements { get; internal set; } = Array.Empty<PolicyEnumerationElementInfo>();

        /// <summary>
        /// Gets the list elements of the policy.
        /// </summary>
        public IReadOnlyList<PolicyListElementInfo> ListElements { get; internal set; } = Array.Empty<PolicyListElementInfo>();

        /// <summary>
        /// Gets the long decimal elements of the policy.
        /// </summary>
        public IReadOnlyList<PolicyLongDecimalElementInfo> LongDecimalElements { get; internal set; } = Array.Empty<PolicyLongDecimalElementInfo>();

        /// <summary>
        /// Gets the multi-text elements of the policy.
        /// </summary>
        public IReadOnlyList<PolicyMultiTextElementInfo> MultiTextElements { get; internal set; } = Array.Empty<PolicyMultiTextElementInfo>();

        /// <summary>
        /// Gets the text elements of the policy.
        /// </summary>
        public IReadOnlyList<PolicyTextElementInfo> TextElements { get; internal set; } = Array.Empty<PolicyTextElementInfo>();

        /// <summary>
        /// Gets or sets the supported on definition info of the policy.
        /// </summary>
        public SupportedOnDefinitionInfo SupportedOn { get; internal set; } = default;

        /// <summary>
        /// Gets the category display names of the policy.
        /// </summary>
        /// <param name="catalog">The policy definition catalog info.</param>
        /// <param name="info">The policy resource info.</param>
        /// <returns>The category display names.</returns>
        public IReadOnlyList<string> GetCategoryDisplayNames(PolicyDefinitionCatalogInfo catalog, PolicyResourceInfo info)
            => new ReadOnlyCollection<string>(CategoryKeys.Select(x => GetCategoryDisplayName(x, catalog, info)).ToArray());

        private string GetCategoryDisplayName(EntityReference categoryKey, PolicyDefinitionCatalogInfo catalog, PolicyResourceInfo info)
        {
            var prefix = categoryKey.Prefix;
            var actualCategoryKey = categoryKey.Key;

            if (catalog != null)
            {
                if (catalog.TryGetFallbackResourceByPrefix(prefix, out var foundResource) && foundResource != null)
                {
                    if (foundResource.StringTable.TryGetValue(actualCategoryKey, out var externalVal) && externalVal != null)
                        return externalVal;
                }
            }

            if (info.StringTable.TryGetValue(actualCategoryKey, out var localVal) && localVal != null)
                return localVal;

            return categoryKey.ToString();
        }

        /// <summary>
        /// Returns a string that represents the current policy.
        /// </summary>
        /// <returns>A string that represents the current policy.</returns>
        public override string ToString()
            => Resolved ? $"[{string.Join(" > ", BaseCategoryDisplayNames)}] {BaseDisplayName} - {BaseExplainText}" : $"[{string.Join(" > ", CategoryKeys)}] {DisplayNameKey} - {ExplainTextKey}";

        /// <summary>
        /// Gets or sets a value indicating whether the policy is resolved.
        /// </summary>
        public bool Resolved { get; private set; }

        /// <summary>
        /// Resolves the policy using the specified catalog and policy definition info.
        /// </summary>
        /// <param name="catalog">The policy definition catalog info.</param>
        /// <param name="policyDefinitionInfo">The policy definition info.</param>
        public void Resolve(PolicyDefinitionCatalogInfo catalog, PolicyDefinitionInfo policyDefinitionInfo)
        {
            if (Resolved)
                return;

            if (policyDefinitionInfo.TryGetFallbackResource(out var foundResource) && foundResource != null)
            {
                if (DisplayNameKey.TryResolveStringFromFallbackCulture(policyDefinitionInfo, out var foundDisplayName) && foundDisplayName != null)
                    BaseDisplayName = foundDisplayName;

                if (ExplainTextKey.TryResolveStringFromFallbackCulture(policyDefinitionInfo, out var foundExplainText) && foundExplainText != null)
                    BaseExplainText = foundExplainText;

                var baseCategoryDisplayNames = new List<string>(CategoryKeys.Count);
                foreach (var eachCategoryKey in CategoryKeys)
                    baseCategoryDisplayNames.Add(GetCategoryDisplayName(eachCategoryKey, catalog, foundResource));
                BaseCategoryDisplayNames = baseCategoryDisplayNames;
            }

            foreach (var eachEnumerationElement in EnumerationElements)
                eachEnumerationElement.Resolve(catalog, policyDefinitionInfo);

            if (SupportedOnKey != null)
            {
                if (string.IsNullOrWhiteSpace(SupportedOnKey.Prefix))
                {
                    if (policyDefinitionInfo.TryGetSupportedOnDefinition(SupportedOnKey.Key, out SupportedOnDefinitionInfo info) && info != null)
                        SupportedOn = info;
                }
                else
                {
                    if (catalog.TryGetSupportedOnByPrefix(SupportedOnKey.Prefix, SupportedOnKey.Key, out SupportedOnDefinitionInfo info) && info != null)
                        SupportedOn = info;
                }
            }

            Resolved = true;
        }
    }
}
