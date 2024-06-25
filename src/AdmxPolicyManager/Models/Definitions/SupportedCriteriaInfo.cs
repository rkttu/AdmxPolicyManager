namespace AdmxPolicyManager.Models.Definitions
{
    /// <summary>
    /// Represents information about the supported criteria.
    /// </summary>
    public sealed class SupportedCriteriaInfo
    {
        internal SupportedCriteriaInfo() { }

        /// <summary>
        /// Gets or sets the supported on type.
        /// </summary>
        public SupportedOnType SupportedOn { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the reference ID.
        /// </summary>
        public string RefId { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the maximum version.
        /// </summary>
        public uint? MaxVersion { get; internal set; } = default;

        /// <summary>
        /// Gets or sets the minimum version.
        /// </summary>
        public uint? MinVersion { get; internal set; } = default;

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            switch (SupportedOn)
            {
                case SupportedOnType.None:
                    return "(none)";
                case SupportedOnType.Range:
                    return $"{MinVersion} <= {RefId} <= {MaxVersion}";
                case SupportedOnType.Reference:
                    return $"== {RefId}";
                default:
                    return "(unknown)";
            }
        }
    }
}
