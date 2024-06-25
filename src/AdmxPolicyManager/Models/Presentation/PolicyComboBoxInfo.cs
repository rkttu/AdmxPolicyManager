using AdmxPolicyManager.Contracts.Presentation;
using System;
using System.Collections.Generic;

namespace AdmxPolicyManager.Models.Presentation
{
    /// <summary>
    /// Represents the information of a policy combo box control in the presentation layer.
    /// </summary>
    public sealed class PolicyComboBoxInfo : IPresentationControlHasRefId
    {
        internal PolicyComboBoxInfo() { }

        /// <summary>
        /// Gets or sets the reference ID of the policy combo box control.
        /// </summary>
        public string RefId { get; internal set; } = default;

        /// <summary>
        /// Gets or sets a value indicating whether the suggestions in the combo box should be sorted or not.
        /// </summary>
        public bool NoSort { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the label of the policy combo box control.
        /// </summary>
        public string Label { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the default value of the policy combo box control.
        /// </summary>
        public string Default { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the list of suggestions for the policy combo box control.
        /// </summary>
        public IReadOnlyList<string> Suggestions { get; internal set; } = Array.Empty<string>();
    }
}
