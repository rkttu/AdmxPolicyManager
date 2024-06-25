using AdmxParser;
using AdmxParser.Serialization;
using System.Linq;

namespace AdmxPolicyManager.Models.Policies
{
    /// <summary>
    /// Represents a policy registry value (including registry key path, value name, and value.)
    /// </summary>
    public sealed class PolicyRegistryValue
    {
        internal static PolicyRegistryValue GetGroupPolicyRegistryValueFromBooleanElement(
            string registryKeyPath, bool? defaultValue, BooleanElement element)
        {
            if (!defaultValue.HasValue)
                return null;
            if (defaultValue.Value && element.trueValue == null)
                return null;
            if (!defaultValue.Value && element.falseValue == null)
                return null;

            return new PolicyRegistryValue(
                registryKeyPath, element.valueName, defaultValue.Value ? element.trueValue : element.falseValue);
        }

        internal static PolicyRegistryValue GetGroupPolicyRegistryValueFromDecimalElement(
            string registryKeyPath, uint? defaultValue, DecimalElement element)
        {
            if (!defaultValue.HasValue)
                return null;

            return new PolicyRegistryValue(
                registryKeyPath, element.valueName, element.storeAsText ? $"{defaultValue}".ToAdmxValue() : defaultValue.ToAdmxValue());
        }

        internal static PolicyRegistryValue GetGroupPolicyRegistryValueFromEnumerationElement(
            string registryKeyPath, int defaultItemIndex, EnumerationElement element)
        {
            var defaultValue = element.item.ElementAtOrDefault(defaultItemIndex)?.value;

            if (defaultValue == null)
                return null;

            return new PolicyRegistryValue(
                registryKeyPath, element.valueName, defaultValue);
        }

        internal static PolicyRegistryValue GetGroupPolicyRegistryValueFromLongDecimalElement(
            string registryKeyPath, ulong? defaultValue, LongDecimalElement element)
        {
            if (!defaultValue.HasValue)
                return null;

            return new PolicyRegistryValue(
                registryKeyPath, element.valueName, element.storeAsText ? $"{defaultValue}".ToAdmxValue() : defaultValue.Value.ToAdmxValue());
        }

        internal static PolicyRegistryValue GetGroupPolicyRegistryValueFromMultiTextElement(
            string registryKeyPath, multiTextElement element)
        {
            return new PolicyRegistryValue(registryKeyPath, element.valueName, string.Empty.ToAdmxValue());
        }

        internal static PolicyRegistryValue GetGroupPolicyRegistryValueFromTextElement(
            string registryKeyPath, string defaultValue, TextElement element)
        {
            if (defaultValue == null)
                return null;

            return new PolicyRegistryValue(registryKeyPath, element.valueName, (defaultValue ?? string.Empty).ToAdmxValue());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PolicyRegistryValue"/> class.
        /// </summary>
        /// <param name="registryKeyPath">The registry key path.</param>
        /// <param name="registryValueName">The registry value name.</param>
        /// <param name="value">The value.</param>
        public PolicyRegistryValue(string registryKeyPath, string registryValueName, Value value)
        {
            _registryKeyPath = registryKeyPath;
            _registryValueName = registryValueName;
            _value = value;

            switch (value?.Item)
            {
                case ValueDecimal vd:
                    _isDecimalValue = true;
                    break;
                case ValueLongDecimal vld:
                    _isLongDecimalValue = true;
                    break;
                case ValueDelete vdel:
                    _isDeleteValue = true;
                    break;
                case string s:
                    _isStringValue = true;
                    break;
                default:
                    throw new GroupPolicyManagementException($"Unsupported element type '{(value?.Item?.GetType()?.ToString() ?? "(null)")}' found.");
            }
        }

        private readonly string _registryKeyPath;
        private readonly string _registryValueName;
        private readonly Value _value;
        private readonly bool _isDecimalValue;
        private readonly bool _isLongDecimalValue;
        private readonly bool _isDeleteValue;
        private readonly bool _isStringValue;

        /// <summary>
        /// Gets the registry key path.
        /// </summary>
        public string RegistryKeyPath => _registryKeyPath;

        /// <summary>
        /// Gets the registry value name.
        /// </summary>
        public string RegistryValueName => _registryValueName;

        /// <summary>
        /// Gets the value.
        /// </summary>
        public Value Value => _value;

        /// <summary>
        /// Gets a value indicating whether this instance is decimal value.
        /// </summary>
        public bool IsDecimalValue => _isDecimalValue;

        /// <summary>
        /// Gets a value indicating whether this instance is long decimal value.
        /// </summary>
        public bool IsLongDecimalValue => _isLongDecimalValue;

        /// <summary>
        /// Gets a value indicating whether this instance is delete value.
        /// </summary>
        public bool IsDeleteValue => _isDeleteValue;

        /// <summary>
        /// Gets a value indicating whether this instance is string value.
        /// </summary>
        public bool IsStringValue => _isStringValue;

        /// <summary>
        /// Ensures this instance is not delete value.
        /// </summary>
        /// <returns>The current instance.</returns>
        /// <exception cref="GroupPolicyManagementException">Thrown when the value is reserved for delete operation.</exception>
        public PolicyRegistryValue EnsureIsNotDeleteValue()
        {
            if (_isDeleteValue)
                throw new GroupPolicyManagementException("This value is reserved for delete operation.");
            return this;
        }

        /// <summary>
        /// Gets the decimal value.
        /// </summary>
        /// <returns>The decimal value.</returns>
        public uint GetDecimalValue()
            => ((ValueDecimal)_value.Item).value;

        /// <summary>
        /// Gets the long decimal value.
        /// </summary>
        /// <returns>The long decimal value.</returns>
        public ulong GetLongDecimalValue()
            => ((ValueLongDecimal)_value.Item).value;

        /// <summary>
        /// Gets the string value.
        /// </summary>
        /// <returns>The string value.</returns>
        public string GetStringValue()
            => (string)_value.Item;

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
            => $"{_registryKeyPath}, {(string.IsNullOrWhiteSpace(_registryValueName) ? "(Default)" : _registryValueName)}: {(_isDeleteValue ? "(Delete)" : _value.GetStringExpression(true))}";
    }
}
