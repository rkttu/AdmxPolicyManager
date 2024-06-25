using AdmxParser.Serialization;
using AdmxPolicyManager.Contracts.Policies;
using AdmxPolicyManager.Models;
using AdmxPolicyManager.Models.Elements;
using AdmxPolicyManager.Models.Policies;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

// Please do not update the namespace for convience of usage in the consumer projects.
namespace AdmxPolicyManager
{
    /// <summary>
    /// Provides extension methods for the <see cref="PolicyInfoBase"/> class.
    /// </summary>
    public static class PolicyElementExtensions
    {
        /// <summary>
        /// Gets the elements of the policy.
        /// </summary>
        /// <param name="policy">The policy.</param>
        /// <returns>The list of key-value pairs representing the elements and their types.</returns>
        public static IReadOnlyList<KeyValuePair<string, PolicyElementType>> GetElements(this PolicyInfoBase policy)
            => new ReadOnlyCollection<KeyValuePair<string, PolicyElementType>>(policy.Elements.Select(x => new KeyValuePair<string, PolicyElementType>(x.Id, x.Type)).ToArray());

        /// <summary>
        /// Gets the names of the elements in the policy.
        /// </summary>
        /// <param name="policy">The policy.</param>
        /// <returns>The list of element names.</returns>
        public static IReadOnlyList<string> GetElementNames(this PolicyInfoBase policy)
            => new ReadOnlyCollection<string>(policy.Elements.Select(x => x.Id).ToArray());

        /// <summary>
        /// Gets the information of the specified element in the policy.
        /// </summary>
        /// <param name="policy">The policy.</param>
        /// <param name="elementName">The name of the element.</param>
        /// <returns>The element information.</returns>
        public static IElementInfo GetElementInfo(this PolicyInfoBase policy, string elementName)
            => policy.Elements.FirstOrDefault(x => string.Equals(elementName, x.Id, StringComparison.Ordinal));

        /// <summary>
        /// Gets the type of the specified element in the policy.
        /// </summary>
        /// <param name="policy">The policy.</param>
        /// <param name="elementName">The name of the element.</param>
        /// <returns>The element type.</returns>
        public static PolicyElementType GetElementType(this PolicyInfoBase policy, string elementName)
            => policy.GetElementInfo(elementName)?.Type ?? default;

        /// <summary>
        /// Gets the enumeration IDs of the specified element in the policy.
        /// </summary>
        /// <param name="policy">The policy.</param>
        /// <param name="elementName">The name of the element.</param>
        /// <returns>The list of enumeration IDs, display names, and values.</returns>
        public static IReadOnlyList<Tuple<string, string, Value>> GetElementEnumIds(this PolicyInfoBase policy, string elementName)
        {
            var element = policy.GetElementInfo(elementName) as PolicyEnumerationElementInfo;
            if (element == null)
                return Array.Empty<Tuple<string, string, Value>>();
            return element.Items.Select(x => new Tuple<string, string, Value>(x?.Id, x?.BaseDisplayName, x?.Value)).ToArray();
        }

        /// <summary>
        /// Gets the value of the specified element in the policy.
        /// </summary>
        /// <param name="policy">The policy.</param>
        /// <param name="elementName">The name of the element.</param>
        /// <param name="userOrGroupSid">The SID of the user or group.</param>
        /// <returns>The value of the element.</returns>
        public static SingleOrMultiple<Value> GetElementValue(this PolicyInfoBase policy, string elementName, string userOrGroupSid)
        {
            var section = policy is MachinePolicyInfo ? PolicySection.Machine : PolicySection.User;
            var element = policy.GetElementInfo(elementName);

            if (element == null)
                return new SingleOrMultiple<Value>();

            switch (element.Type)
            {
                case PolicyElementType.Boolean:
                    PolicyBooleanElementInfo be = (PolicyBooleanElementInfo)element;
                    if (be.RegistryValue != null)
                        return new SingleOrMultiple<Value>(be.RegistryValue.ReadRawValue(section, userOrGroupSid));
                    break;
                case PolicyElementType.Decimal:
                    PolicyDecimalElementInfo de = (PolicyDecimalElementInfo)element;
                    if (de.RegistryValue != null)
                        return new SingleOrMultiple<Value>(de.RegistryValue.ReadRawValue(section, userOrGroupSid));
                    break;
                case PolicyElementType.Enumeration:
                    PolicyEnumerationElementInfo ee = (PolicyEnumerationElementInfo)element;
                    if (ee.RegistryValue != null)
                        return new SingleOrMultiple<Value>(ee.RegistryValue.ReadRawValue(section, userOrGroupSid));
                    break;
                case PolicyElementType.List:
                    PolicyListElementInfo le = (PolicyListElementInfo)element;
                    return new SingleOrMultiple<Value>(le.ReadRawValues(section, userOrGroupSid).Select(x => x.Value).ToArray());
                case PolicyElementType.LongDecimal:
                    PolicyLongDecimalElementInfo lde = (PolicyLongDecimalElementInfo)element;
                    if (lde.RegistryValue != null)
                        return new SingleOrMultiple<Value>(lde.RegistryValue.ReadRawValue(section, userOrGroupSid));
                    break;
                case PolicyElementType.MultiText:
                    PolicyMultiTextElementInfo mte = (PolicyMultiTextElementInfo)element;
                    if (mte.RegistryValue != null)
                        return new SingleOrMultiple<Value>(mte.RegistryValue.ReadRawValue(section, userOrGroupSid));
                    break;
                case PolicyElementType.Text:
                    PolicyTextElementInfo te = (PolicyTextElementInfo)element;
                    if (te.RegistryValue != null)
                        return new SingleOrMultiple<Value>(te.RegistryValue.ReadRawValue(section, userOrGroupSid));
                    break;
                default:
                    if (element.RegistryValue != null)
                        return new SingleOrMultiple<Value>(element.RegistryValue.ReadRawValue(section, userOrGroupSid));
                    break;
            }

            return new SingleOrMultiple<Value>();
        }
    }
}
