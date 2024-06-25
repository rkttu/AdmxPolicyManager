namespace AdmxPolicyManager.Models.Definitions
{
    /// <summary>
    /// Represents the supported on type for a policy definition.
    /// </summary>
    public enum SupportedOnType : int
    {
        /// <summary>
        /// No supported on type specified.
        /// </summary>
        None = default,

        /// <summary>
        /// Supported on type is a range.
        /// </summary>
        Range,

        /// <summary>
        /// Supported on type is a reference.
        /// </summary>
        Reference,
    }
}
