using AdmxPolicyManager.Contracts.Presentation;

namespace AdmxPolicyManager.Models.Presentation
{
    /// <summary>
    /// Represents the information of a text control in the presentation.
    /// </summary>
    public sealed class TextInfo : IPresentationControlInfo
    {
        internal TextInfo() { }

        /// <summary>
        /// Gets or sets the label of the text control.
        /// </summary>
        public string Label { get; internal set; } = default;
    }
}
