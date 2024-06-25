using AdmxPolicyManager.Contracts.Presentation;
using System;
using System.Collections.ObjectModel;

namespace AdmxPolicyManager.Models.Presentation
{
    /// <summary>
    /// Represents a collection of presentation controls with reference IDs.
    /// </summary>
    /// <typeparam name="TPresentationControlHasRefId">The type of the presentation control with reference ID.</typeparam>
    public sealed class PresentationControlHasRefIdCollection<TPresentationControlHasRefId> : KeyedCollection<string, TPresentationControlHasRefId>
        where TPresentationControlHasRefId : IPresentationControlHasRefId
    {
        /// <summary>
        /// Gets the key for the specified item.
        /// </summary>
        /// <param name="item">The item to get the key for.</param>
        /// <returns>The key for the specified item.</returns>
        protected override string GetKeyForItem(TPresentationControlHasRefId item)
            => item.RefId;

        /// <summary>
        /// Tries to get the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="output">When this method returns, contains the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter.</param>
        /// <returns><c>true</c> if the collection contains an element with the specified key; otherwise, <c>false</c>.</returns>
        internal bool TryGetValue(string key, out TPresentationControlHasRefId output)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            output = default;
            if (!Contains(key))
                return false;

            output = this[key];
            return true;
        }
    }
}
