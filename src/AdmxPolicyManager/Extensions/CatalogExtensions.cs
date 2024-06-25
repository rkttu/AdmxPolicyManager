using AdmxParser;
using AdmxPolicyManager.Models;
using AdmxPolicyManager.Models.Definitions;
using System;
using System.Threading;
using System.Threading.Tasks;

// Please do not update the namespace for convience of usage in the consumer projects.
namespace AdmxPolicyManager
{
    /// <summary>
    /// Provides extension methods for the <see cref="AdmxDirectory"/> class.
    /// </summary>
    public static class CatalogExtensions
    {
        /// <summary>
        /// Asynchronously retrieves the policy definition catalog information from the specified <see cref="AdmxDirectory"/>.
        /// </summary>
        /// <param name="directory">The <see cref="AdmxDirectory"/> to retrieve the catalog information from.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="PolicyDefinitionCatalogInfo"/>.</returns>
        public static async Task<PolicyDefinitionCatalogInfo> GetCatalogAsync(this AdmxDirectory directory, CancellationToken cancellationToken = default)
        {
            if (directory == null)
                throw new ArgumentNullException(nameof(directory));

            if (!directory.Loaded)
                await directory.LoadAsync(true, cancellationToken).ConfigureAwait(false);

            return directory.LoadPolicyDefinitionInfosFromDirectory();
        }
    }
}
