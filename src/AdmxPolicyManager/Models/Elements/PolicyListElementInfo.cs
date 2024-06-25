using AdmxParser;
using AdmxPolicyManager.Contracts.Policies;
using AdmxPolicyManager.Models.Policies;
using AdmxPolicyManager.Models.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdmxPolicyManager.Models.Elements
{
    /// <summary>
    /// Represents the information of a policy list element.
    /// </summary>
    public sealed class PolicyListElementInfo : IElementInfo<PolicyListBoxInfo>
    {
        internal PolicyListElementInfo() { }

        /// <summary>
        /// Gets or sets the presentation information of the policy list element.
        /// </summary>
        public PolicyListBoxInfo Presentation { get; internal set; } = default;

        /// <summary>
        /// Gets the type of the policy element.
        /// </summary>
        public PolicyElementType Type => PolicyElementType.List;

        /// <summary>
        /// Gets or sets the client extension of the policy list element.
        /// </summary>
        public string ClientExtension { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the ID of the policy list element.
        /// </summary>
        public string Id { get; internal set; } = default;

        /// <summary>
        /// Gets or sets a value indicating whether the policy list element is additive.
        /// </summary>
        public bool Additive { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether the policy list element is expandable.
        /// </summary>
        public bool Expandable { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether the policy list element has an explicit value.
        /// </summary>
        public bool ExplicitValue { get; internal set; }

        /// <summary>
        /// Gets or sets the registry key path of the policy list element.
        /// </summary>
        public string RegistryKeyPath { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the registry value prefix of the policy list element.
        /// </summary>
        public string RegistryValuePrefix { get; internal set; } = default;

        PolicyRegistryValue IElementInfo.RegistryValue => null;

        /// <summary>
        /// Gets the names of the element values based on the specified item count.
        /// </summary>
        /// <param name="itemCount">The number of items in the list.</param>
        /// <returns>The names of the element values.</returns>
        public IReadOnlyList<string> GetElementValueNames(int itemCount)
        {
            if (itemCount < 1)
                return Array.Empty<string>();

            return Enumerable.Range(1, itemCount).Select(i => $"{RegistryValuePrefix}{i}").ToArray();
        }

        /// <summary>
        /// Gets the element values based on the specified item count.
        /// </summary>
        /// <param name="itemCount">The number of items in the list.</param>
        /// <returns>The element values.</returns>
        public IReadOnlyList<PolicyRegistryValue> GetElementValues(int itemCount)
            => GetElementValueNames(itemCount).Select(x => new PolicyRegistryValue(RegistryKeyPath, x, string.Empty.ToAdmxValue())).ToArray();
    }
}
