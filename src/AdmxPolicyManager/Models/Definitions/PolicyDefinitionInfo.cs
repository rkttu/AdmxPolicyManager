using AdmxParser.Serialization;
using AdmxPolicyManager.Contracts.Presentation;
using AdmxPolicyManager.Models.Policies;
using AdmxPolicyManager.Models.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace AdmxPolicyManager.Models.Definitions
{
    /// <summary>
    /// Represents information about a policy definition.
    /// </summary>
    public sealed class PolicyDefinitionInfo : IResolvable
    {
        internal PolicyDefinitionInfo() { }

        /// <summary>
        /// Gets or sets the ADMX file path.
        /// </summary>
        public string AdmxFilePath { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the target namespace of the policy definition.
        /// </summary>
        public PolicyNamespaceAssociation TargetNamespace { get; internal set; } = default;

        /// <summary>
        /// Gets the list of using namespaces.
        /// </summary>
        public IReadOnlyList<PolicyNamespaceAssociation> UsingNamespaces { get; internal set; } = Array.Empty<PolicyNamespaceAssociation>();

        /// <summary>
        /// Gets the list of policy names.
        /// </summary>
        public IReadOnlyList<string> PolicyNames { get; internal set; } = Array.Empty<string>();

        /// <summary>
        /// Gets the list of machine policies.
        /// </summary>
        public IReadOnlyList<MachinePolicyInfo> MachinePolicies { get; internal set; } = Array.Empty<MachinePolicyInfo>();

        /// <summary>
        /// Gets the list of user policies.
        /// </summary>
        public IReadOnlyList<UserPolicyInfo> UserPolicies { get; internal set; } = Array.Empty<UserPolicyInfo>();

        /// <summary>
        /// Gets the list of policy resources.
        /// </summary>
        public IReadOnlyList<PolicyResourceInfo> Resources { get; internal set; } = Array.Empty<PolicyResourceInfo>();

        /// <summary>
        /// Gets the list of superseded files.
        /// </summary>
        public IReadOnlyList<string> SupersededFiles { get; internal set; } = Array.Empty<string>();

        /// <summary>
        /// Gets the list of supported on products.
        /// </summary>
        public IReadOnlyList<SupportedOnProductInfo> SupportedOnProducts { get; internal set; } = Array.Empty<SupportedOnProductInfo>();

        /// <summary>
        /// Gets the list of supported on target definitions.
        /// </summary>
        public IReadOnlyList<SupportedOnDefinitionInfo> SupportedOnTargetDefinitions { get; internal set; } = Array.Empty<SupportedOnDefinitionInfo>();

        /// <summary>
        /// Gets the list of referenced policy definitions.
        /// </summary>
        public IReadOnlyList<PolicyDefinitionInfo> ReferencedPolicyDefinitions { get; internal set; } = Array.Empty<PolicyDefinitionInfo>();

        /// <summary>
        /// Tries to get the policy resource by culture.
        /// </summary>
        /// <param name="targetCulture">The target culture.</param>
        /// <param name="resource">The policy resource.</param>
        /// <returns><c>true</c> if the policy resource is found; otherwise, <c>false</c>.</returns>
        public bool TryGetResourceByCulture(CultureInfo targetCulture, out PolicyResourceInfo resource)
            => (resource = Resources.FirstOrDefault(x => string.Equals(x.TargetCulture.Name, targetCulture.Name, StringComparison.OrdinalIgnoreCase))) != null;

        /// <summary>
        /// Tries to get the fallback policy resource.
        /// </summary>
        /// <param name="resource">The policy resource.</param>
        /// <returns><c>true</c> if the fallback policy resource is found; otherwise, <c>false</c>.</returns>
        public bool TryGetFallbackResource(out PolicyResourceInfo resource)
            => TryGetResourceByCulture(new CultureInfo("en-US"), out resource);

        /// <summary>
        /// Tries to get the supported on definition.
        /// </summary>
        /// <param name="targetName">The target name.</param>
        /// <param name="info">The supported on definition info.</param>
        /// <returns><c>true</c> if the supported on definition is found; otherwise, <c>false</c>.</returns>
        public bool TryGetSupportedOnDefinition(string targetName, out SupportedOnDefinitionInfo info)
            => (info = SupportedOnTargetDefinitions?.FirstOrDefault(x => string.Equals(x.DefinitionName, targetName, StringComparison.Ordinal))) != null;

        /// <summary>
        /// Tries to get the supported on product.
        /// </summary>
        /// <param name="targetName">The target name.</param>
        /// <param name="info">The supported on product info.</param>
        /// <returns><c>true</c> if the supported on product is found; otherwise, <c>false</c>.</returns>
        public bool TryGetSupportedOnProduct(string targetName, out SupportedOnProductInfo info)
            => (info = SupportedOnProducts?.FirstOrDefault(x => string.Equals(x.ProductName, targetName, StringComparison.OrdinalIgnoreCase))) != null;

        /// <summary>
        /// Returns a string that represents the policy definition info.
        /// </summary>
        /// <returns>A string that represents the policy definition info.</returns>
        public override string ToString()
            => $"{AdmxFilePath}, {MachinePolicies.Count} machine policies and {UserPolicies.Count} user policies found. (Total {MachinePolicies.Count + UserPolicies.Count} policies found.)";

        /// <summary>
        /// Gets or sets a value indicating whether the policy definition is resolved.
        /// </summary>
        public bool Resolved { get; private set; }

        /// <summary>
        /// Resolves the policy definition.
        /// </summary>
        /// <param name="catalog">The policy definition catalog.</param>
        public void Resolve(PolicyDefinitionCatalogInfo catalog)
        {
            if (Resolved)
                return;

            foreach (var eachMachinePolicy in MachinePolicies)
                eachMachinePolicy.Resolve(catalog, this);

            foreach (var eachUserPolicy in UserPolicies)
                eachUserPolicy.Resolve(catalog, this);

            foreach (var eachProductInfo in SupportedOnProducts)
                eachProductInfo.Resolve(catalog, this);

            foreach (var eachDefinition in SupportedOnTargetDefinitions)
                eachDefinition.Resolve(catalog, this);

            Resolved = true;
        }

        void IResolvable.Resolve(PolicyDefinitionCatalogInfo catalog, PolicyDefinitionInfo _)
            => Resolve(catalog);
    }
}
