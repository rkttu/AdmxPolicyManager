namespace AdmxPolicyManager.Contracts.Policies
{
    /// <summary>
    /// Represents the type of a policy element.
    /// </summary>
    public enum PolicyElementType : int
    {
        /// <summary>
        /// No specific type.
        /// </summary>
        None = default,

        /// <summary>
        /// Boolean type.
        /// </summary>
        Boolean,

        /// <summary>
        /// Decimal type.
        /// </summary>
        Decimal,

        /// <summary>
        /// Enumeration type.
        /// </summary>
        Enumeration,

        /// <summary>
        /// List type.
        /// </summary>
        List,

        /// <summary>
        /// Long decimal type.
        /// </summary>
        LongDecimal,

        /// <summary>
        /// Multi-line text type.
        /// </summary>
        MultiText,

        /// <summary>
        /// Single-line text type.
        /// </summary>
        Text,
    }
}
