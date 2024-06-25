namespace AdmxPolicyManager.Contracts.Presentation
{
    /// <summary>
    /// Represents a presentation control that has a reference ID.
    /// </summary>
    public interface IPresentationControlHasRefId : IPresentationControlInfo
    {
        /// <summary>
        /// Gets the reference ID of the presentation control.
        /// </summary>
        string RefId { get; }
    }
}
