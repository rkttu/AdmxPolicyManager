using AdmxPolicyManager.Contracts.Presentation;
using AdmxPolicyManager.Models.Resources;
using System;

namespace AdmxPolicyManager.Models.Definitions
{
    /// <summary>
    /// Represents information about the supported product versions for a policy definition.
    /// </summary>
    public sealed class SupportedOnProductInfo : IResolvable
    {
        internal SupportedOnProductInfo() { }

        /// <summary>
        /// Gets or sets the base product display name.
        /// </summary>
        public string BaseProductDisplayName { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the base major version name.
        /// </summary>
        public string BaseMajorVersionName { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the base minor version name.
        /// </summary>
        public string BaseMinorVersionName { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the version of the supported product.
        /// </summary>
        public Version Version { get; internal set; } = default;

        /// <summary>
        /// Gets or sets a value indicating whether the supported product has a major version.
        /// </summary>
        public bool HasMajorVersion { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the supported product has a minor version.
        /// </summary>
        public bool HasMinorVersion { get; set; }

        /// <summary>
        /// Gets or sets the name of the supported product.
        /// </summary>
        public string ProductName { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the name of the major version of the supported product.
        /// </summary>
        public string MajorVersionName { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the name of the minor version of the supported product.
        /// </summary>
        public string MinorVersionName { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the resource key reference for the product display name.
        /// </summary>
        public ResourceKeyReference ProductDisplayNameKey { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the resource key reference for the major version display name.
        /// </summary>
        public ResourceKeyReference MajorVersionDisplayNameKey { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the index of the major version.
        /// </summary>
        public uint MajorVersionIndex { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the resource key reference for the minor version display name.
        /// </summary>
        public ResourceKeyReference MinorVersionDisplayNameKey { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the index of the minor version.
        /// </summary>
        public uint? MinorVersionIndex { get; internal set; } = default;

        /// <summary>
        /// Gets a value indicating whether the supported product information has been resolved.
        /// </summary>
        public bool Resolved { get; private set; }

        /// <summary>
        /// Resolves the supported product information using the specified catalog and policy definition information.
        /// </summary>
        /// <param name="catalog">The policy definition catalog information.</param>
        /// <param name="policyDefinitionInfo">The policy definition information.</param>
        public void Resolve(PolicyDefinitionCatalogInfo catalog, PolicyDefinitionInfo policyDefinitionInfo)
        {
            if (Resolved)
                return;

            if (policyDefinitionInfo.TryGetFallbackResource(out var foundResource) && foundResource != null)
            {
                if (ProductDisplayNameKey.TryResolveStringFromFallbackCulture(policyDefinitionInfo, out var productDisplayName) && productDisplayName != null)
                    BaseProductDisplayName = productDisplayName;

                if (MajorVersionDisplayNameKey.TryResolveStringFromFallbackCulture(policyDefinitionInfo, out var majorVersionDisplayName) && majorVersionDisplayName != null)
                    BaseMajorVersionName = majorVersionDisplayName;

                if (MinorVersionDisplayNameKey != null)
                {
                    if (MinorVersionDisplayNameKey.TryResolveStringFromFallbackCulture(policyDefinitionInfo, out var minorVersionDisplayName) && minorVersionDisplayName != null)
                        BaseMinorVersionName = minorVersionDisplayName;
                }
                else
                    BaseMinorVersionName = string.Empty;
            }

            Resolved = true;
        }
    }
}
