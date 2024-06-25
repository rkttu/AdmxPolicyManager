using AdmxPolicyManager.Contracts.Presentation;
using AdmxPolicyManager.Models.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace AdmxPolicyManager.Models.Definitions
{
    /// <summary>
    /// Represents the catalog information for policy definitions.
    /// </summary>
    public sealed class PolicyDefinitionCatalogInfo : IResolvable
    {
        internal PolicyDefinitionCatalogInfo() { }

        /// <summary>
        /// Gets or sets the path to the ADMX directory.
        /// </summary>
        public string AdmxDirectoryPath { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the list of policy definitions.
        /// </summary>
        public IReadOnlyList<PolicyDefinitionInfo> PolicyDefinitions { get; internal set; } = Array.Empty<PolicyDefinitionInfo>();

        /// <summary>
        /// Gets or sets the list of prefixes.
        /// </summary>
        public IReadOnlyList<string> Prefixes { get; internal set; } = Array.Empty<string>();

        /// <summary>
        /// Gets or sets the list of namespaces.
        /// </summary>
        public IReadOnlyList<string> Namespaces { get; internal set; } = Array.Empty<string>();

        /// <summary>
        /// Tries to get the policy definition by prefix.
        /// </summary>
        /// <param name="prefix">The prefix to search for.</param>
        /// <param name="policy">The policy definition.</param>
        /// <returns><c>true</c> if the policy definition is found; otherwise, <c>false</c>.</returns>
        public bool TryGetPolicyDefinitionByPrefix(string prefix, out PolicyDefinitionInfo policy)
            => (policy = PolicyDefinitions.FirstOrDefault(x => string.Equals(prefix, x.TargetNamespace.prefix, StringComparison.Ordinal))) != null;

        /// <summary>
        /// Tries to get the policy definition by namespace.
        /// </summary>
        /// <param name="namespace">The namespace to search for.</param>
        /// <param name="policy">The policy definition.</param>
        /// <returns><c>true</c> if the policy definition is found; otherwise, <c>false</c>.</returns>
        public bool TryGetPolicyDefinitionByNamespace(string @namespace, out PolicyDefinitionInfo policy)
            => (policy = PolicyDefinitions.FirstOrDefault(x => string.Equals(@namespace, x.TargetNamespace.@namespace, StringComparison.Ordinal))) != null;

        /// <summary>
        /// Tries to get the policy resource by prefix and target culture.
        /// </summary>
        /// <param name="prefix">The prefix to search for.</param>
        /// <param name="targetCulture">The target culture.</param>
        /// <param name="resource">The policy resource.</param>
        /// <returns><c>true</c> if the policy resource is found; otherwise, <c>false</c>.</returns>
        public bool TryGetPolicyResourceByPrefix(string prefix, CultureInfo targetCulture, out PolicyResourceInfo resource)
        {
            resource = default;
            if (TryGetPolicyDefinitionByPrefix(prefix, out var foundPolicy) && foundPolicy != null)
                return foundPolicy.TryGetResourceByCulture(targetCulture, out resource);
            return false;
        }

        /// <summary>
        /// Tries to get the policy resource by namespace and target culture.
        /// </summary>
        /// <param name="namespace">The namespace to search for.</param>
        /// <param name="targetCulture">The target culture.</param>
        /// <param name="resource">The policy resource.</param>
        /// <returns><c>true</c> if the policy resource is found; otherwise, <c>false</c>.</returns>
        public bool TryGetPolicyResourceByNamespace(string @namespace, CultureInfo targetCulture, out PolicyResourceInfo resource)
        {
            resource = default;
            if (TryGetPolicyDefinitionByNamespace(@namespace, out var foundPolicy) && foundPolicy != null)
                return foundPolicy.TryGetResourceByCulture(targetCulture, out resource);
            return false;
        }

        /// <summary>
        /// Tries to get the fallback policy resource by prefix.
        /// </summary>
        /// <param name="prefix">The prefix to search for.</param>
        /// <param name="resource">The fallback policy resource.</param>
        /// <returns><c>true</c> if the fallback policy resource is found; otherwise, <c>false</c>.</returns>
        public bool TryGetFallbackResourceByPrefix(string prefix, out PolicyResourceInfo resource)
        {
            resource = default;
            if (TryGetPolicyDefinitionByPrefix(prefix, out var foundPolicy) && foundPolicy != null)
                return foundPolicy.TryGetFallbackResource(out resource);
            return false;
        }

        /// <summary>
        /// Tries to get the fallback policy resource by namespace.
        /// </summary>
        /// <param name="namespace">The namespace to search for.</param>
        /// <param name="resource">The fallback policy resource.</param>
        /// <returns><c>true</c> if the fallback policy resource is found; otherwise, <c>false</c>.</returns>
        public bool TryGetFallbackResourceByNamespace(string @namespace, out PolicyResourceInfo resource)
        {
            resource = default;
            if (TryGetPolicyDefinitionByNamespace(@namespace, out var foundPolicy) && foundPolicy != null)
                return foundPolicy.TryGetFallbackResource(out resource);
            return false;
        }

        /// <summary>
        /// Tries to get the supported on definition by prefix and target name.
        /// </summary>
        /// <param name="prefix">The prefix to search for.</param>
        /// <param name="targetName">The target name.</param>
        /// <param name="definition">The supported on definition.</param>
        /// <returns><c>true</c> if the supported on definition is found; otherwise, <c>false</c>.</returns>
        public bool TryGetSupportedOnByPrefix(string prefix, string targetName, out SupportedOnDefinitionInfo definition)
        {
            definition = default;
            if (TryGetPolicyDefinitionByPrefix(prefix, out var foundPolicy) && foundPolicy != null)
                return foundPolicy.TryGetSupportedOnDefinition(targetName, out definition);
            return false;
        }

        /// <summary>
        /// Tries to get the supported on definition by namespace and target name.
        /// </summary>
        /// <param name="namespace">The namespace to search for.</param>
        /// <param name="targetName">The target name.</param>
        /// <param name="definition">The supported on definition.</param>
        /// <returns><c>true</c> if the supported on definition is found; otherwise, <c>false</c>.</returns>
        public bool TryGetSupportedOnByNamespace(string @namespace, string targetName, out SupportedOnDefinitionInfo definition)
        {
            definition = default;
            if (TryGetPolicyDefinitionByNamespace(@namespace, out var foundPolicy) && foundPolicy != null)
                return foundPolicy.TryGetSupportedOnDefinition(targetName, out definition);
            return false;
        }

        /// <summary>
        /// Returns a string representation of the policy definition catalog info.
        /// </summary>
        /// <returns>A string representation of the policy definition catalog info.</returns>
        public override string ToString()
            => $"{AdmxDirectoryPath}, {PolicyDefinitions.Count} policy definition(s) found.";

        /// <summary>
        /// Gets or sets a value indicating whether the policy definition catalog info is resolved.
        /// </summary>
        public bool Resolved { get; private set; }

        /// <summary>
        /// Resolves the policy definition catalog info.
        /// </summary>
        public void Resolve()
        {
            if (Resolved)
                return;

            foreach (var eachLoadedPolicyInfo in PolicyDefinitions)
                eachLoadedPolicyInfo.Resolve(this);

            Resolved = true;
        }

        void IResolvable.Resolve(PolicyDefinitionCatalogInfo _, PolicyDefinitionInfo __)
            => this.Resolve();
    }
}
