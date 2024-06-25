using AdmxPolicyManager.Models.Presentation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace AdmxPolicyManager.Models.Resources
{
    /// <summary>
    /// Represents information about a policy resource.
    /// </summary>
    public sealed class PolicyResourceInfo
    {
        internal PolicyResourceInfo() { }

        /// <summary>
        /// Gets or sets the target culture of the policy resource.
        /// </summary>
        public CultureInfo TargetCulture { get; internal set; } = new CultureInfo("en-US");

        /// <summary>
        /// Gets or sets the list of string keys in the policy resource.
        /// </summary>
        public IReadOnlyList<string> StringKeys { get; internal set; } = Array.Empty<string>();

        /// <summary>
        /// Gets or sets the string table of the policy resource.
        /// </summary>
        public IReadOnlyDictionary<string, string> StringTable { get; internal set; } = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());

        /// <summary>
        /// Gets or sets the list of policy presentations in the policy resource.
        /// </summary>
        public IReadOnlyList<PolicyPresentationInfo> Presentations { get; internal set; } = Array.Empty<PolicyPresentationInfo>();

        /// <summary>
        /// Gets a value indicating whether the target culture is the fallback culture (en-US).
        /// </summary>
        public bool IsFallbackCulture => string.Equals(TargetCulture?.Name ?? string.Empty, "en-US", StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Tries to get the string value associated with the specified key from the string table.
        /// </summary>
        /// <param name="key">The key of the string value to retrieve.</param>
        /// <param name="foundString">When this method returns, contains the string value associated with the specified key, if the key is found; otherwise, an empty string.</param>
        /// <returns><c>true</c> if the string value is found in the string table; otherwise, <c>false</c>.</returns>
        public bool TryGetStringByKey(string key, out string foundString)
            => (StringTable.TryGetValue(key, out foundString) && foundString != null);

        /// <summary>
        /// Tries to get the policy presentation associated with the specified key from the list of policy presentations.
        /// </summary>
        /// <param name="key">The key of the policy presentation to retrieve.</param>
        /// <param name="foundPresentation">When this method returns, contains the policy presentation associated with the specified key, if the key is found; otherwise, <c>null</c>.</param>
        /// <returns><c>true</c> if the policy presentation is found in the list of policy presentations; otherwise, <c>false</c>.</returns>
        public bool TryGetPresentationByKey(string key, out PolicyPresentationInfo foundPresentation)
            => (foundPresentation = Presentations.FirstOrDefault(x => string.Equals(x.Id, key, StringComparison.Ordinal))) != null;
    }
}
