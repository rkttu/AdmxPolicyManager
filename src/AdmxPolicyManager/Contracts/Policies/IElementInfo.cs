using AdmxPolicyManager.Contracts.Presentation;
using AdmxPolicyManager.Models.Policies;

namespace AdmxPolicyManager.Contracts.Policies
{
    /// <summary>
    /// Represents an element information.
    /// </summary>
    public interface IElementInfo
    {
        /// <summary>
        /// Gets the ID of the element.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets the type of the element.
        /// </summary>
        PolicyElementType Type { get; }

        /// <summary>
        /// Gets the registry value of the element.
        /// </summary>
        PolicyRegistryValue RegistryValue { get; }
    }

    /// <summary>
    /// Represents an element information with presentation control information.
    /// </summary>
    /// <typeparam name="TPresentationControlInfo">The type of the presentation control information.</typeparam>
    public interface IElementInfo<TPresentationControlInfo> : IElementInfo
        where TPresentationControlInfo : IPresentationControlInfo
    {
        /// <summary>
        /// Gets the presentation control information of the element.
        /// </summary>
        TPresentationControlInfo Presentation { get; }
    }
}
