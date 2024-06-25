using AdmxPolicyManager.Models.Definitions;

namespace AdmxPolicyManager.Contracts.Presentation
{
    /// <summary>
    /// Represents an interface for a resolvable object.
    /// </summary>
    public interface IResolvable
    {
        /// <summary>
        /// Gets a value indicating whether the object is resolved.
        /// </summary>
        bool Resolved { get; }

        /// <summary>
        /// Resolves the object using the specified catalog and policy definition information.
        /// </summary>
        /// <param name="catalog">The policy definition catalog.</param>
        /// <param name="policyDefinitionInfo">The policy definition information.</param>
        void Resolve(PolicyDefinitionCatalogInfo catalog, PolicyDefinitionInfo policyDefinitionInfo);
    }
}
