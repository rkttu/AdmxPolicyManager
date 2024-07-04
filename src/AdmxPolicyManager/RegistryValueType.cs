namespace AdmxPolicyManager;

/// <summary>
/// Represents the type of value stored in the registry.
/// </summary>
public enum RegistryValueType : int
{
    /// <summary>
    /// No value type.
    /// </summary>
    None = 0x0,

    /// <summary>
    /// String value type.
    /// </summary>
    String = 0x1,

    /// <summary>
    /// Expandable string value type.
    /// </summary>
    ExpandString = 0x2,

    /// <summary>
    /// Binary value type.
    /// </summary>
    Binary = 0x3,

    /// <summary>
    /// 32-bit unsigned integer value type.
    /// </summary>
    DWord = 0x4,

    /// <summary>
    /// 32-bit unsigned integer value type (big-endian).
    /// </summary>
    DWordBigEndian = 0x5,

    /// <summary>
    /// Symbolic link value type.
    /// </summary>
    Link = 0x6,

    /// <summary>
    /// Multi-string value type.
    /// </summary>
    MultiString = 0x7,

    /// <summary>
    /// Resource list value type.
    /// </summary>
    ResourceList = 0x8,

    /// <summary>
    /// 64-bit unsigned integer value type.
    /// </summary>
    QWord = 0xb,

    /// <summary>
    /// 64-bit unsigned integer value type (little-endian).
    /// </summary>
    QWordLittleEndian = 0xb,
}
