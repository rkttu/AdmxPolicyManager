using AdmxParser;
using AdmxParser.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace AdmxPolicyManager;

/// <summary>
/// A class that provides extension methods for the <see cref="AdmxContent"/> class.
/// </summary>
public static class AdmxExtensions
{
    private static readonly Lazy<Regex> _regexFactory = new Lazy<Regex>(
        () => new Regex(@"\$\((?<ResourceType>[^.]+)\.(?<ResourceKey>[^\)]+)\)", RegexOptions.Compiled | RegexOptions.IgnoreCase),
        LazyThreadSafetyMode.None);
    
    /// <summary>
    /// Converts the specified <see cref="Value"/> object to an integer.
    /// </summary>
    /// <param name="i">
    /// The integer value to convert.
    /// </param>
    /// <returns>
    /// A new <see cref="Value"/> object that represents the integer value.
    /// </returns>
    public static Value ToAdmxValue(this int i) => new Value() { Item = new ValueDecimal() { value = unchecked((uint)i), }, };

    /// <summary>
    /// Converts the specified <see cref="Value"/> object to an unsigned integer.
    /// </summary>
    /// <param name="ui">
    /// The unsigned integer value to convert.
    /// </param>
    /// <returns>
    /// A new <see cref="Value"/> object that represents the unsigned integer value.
    /// </returns>
    public static Value ToAdmxValue(this uint ui) => new Value() { Item = new ValueDecimal() { value = ui, }, };

    /// <summary>
    /// Converts the specified <see cref="Value"/> object to a long integer.
    /// </summary>
    /// <param name="l">
    /// The long integer value to convert.
    /// </param>
    /// <returns>
    /// A new <see cref="Value"/> object that represents the long integer value.
    /// </returns>
    public static Value ToAdmxValue(this long l) => new Value() { Item = new ValueLongDecimal() { value = unchecked((ulong)l), }, };

    /// <summary>
    /// Converts the specified <see cref="Value"/> object to an unsigned long integer.
    /// </summary>
    /// <param name="ul">
    /// The unsigned long integer value to convert.
    /// </param>
    /// <returns>
    /// A new <see cref="Value"/> object that represents the unsigned long integer value.
    /// </returns>
    public static Value ToAdmxValue(this ulong ul) => new Value() { Item = new ValueLongDecimal() { value = ul, }, };

    /// <summary>
    /// Converts the specified <see cref="Value"/> object to a string.
    /// </summary>
    /// <param name="s">
    /// The string value to convert.
    /// </param>
    /// <returns>
    /// A new <see cref="Value"/> object that represents the string value.
    /// </returns>
    public static Value ToAdxmValue(this string s) => new Value() { Item = s, };

    /// <summary>
    /// Converts the specified object to a <see cref="Value"/> object.
    /// </summary>
    /// <param name="o">
    /// The object to convert.
    /// </param>
    /// <remarks>
    /// The supported types are <see cref="int"/>, <see cref="uint"/>, <see cref="long"/>, <see cref="ulong"/>, and <see cref="string"/>.
    /// Otherwise, a <see cref="NotSupportedException"/> will be thrown.
    /// </remarks>
    /// <returns>
    /// A new <see cref="Value"/> object that represents the object.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// The specified object is not supported.
    /// </exception>
    public static Value ToAdmxValue(this object? o)
    {
        if (o is int i) return i.ToAdmxValue();
        if (o is uint ui) return ui.ToAdmxValue();
        if (o is long l) return l.ToAdmxValue();
        if (o is ulong ul) return ul.ToAdmxValue();
        if (o is string s) return s.ToAdmxValue();
        throw new NotSupportedException($"Selected type '{o?.GetType()?.Name ?? "(null)"}' is not supported.");
    }

    /// <summary>
    /// Determines whether the specified <see cref="Value"/> object is a delete value.
    /// </summary>
    /// <param name="value">
    /// The <see cref="Value"/> object to check.
    /// </param>
    /// <remarks>
    /// A delete value is a special value that indicates the policy should be removed.
    /// </remarks>
    /// <returns>
    /// <see langword="true"/> if the specified <see cref="Value"/> object is a delete value; otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// The specified <see cref="Value"/> object is not a delete value.
    /// </exception>
    public static object GetValue(this Value value)
    {
        if (value.Item is ValueDecimal vd)
            return unchecked((int)vd.value);
        else if (value.Item is ValueLongDecimal vld)
            return unchecked((long)vld.value);
        else if (value.Item is string s)
            return s;
        else if (value.Item is ValueDelete vdel)
            return vdel;
        else
            throw new ArgumentException($"Unknown value type '{value.Item?.GetType()?.Name ?? "(null)"}'.");
    }

    /// <summary>
    /// Gets the AdmxContent matching the namespace.
    /// </summary>
    /// <param name="admxDirectory">
    /// The ADMX directory to check.
    /// </param>
    /// <param name="namespace">
    /// The namespace to match.
    /// </param>
    /// <returns>
    /// AdmxContent matching the namespace.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="admxDirectory"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// ADMX directory is not loaded.
    /// </exception>
    public static AdmxContent? GetAdmxContent(this AdmxDirectory admxDirectory, string @namespace)
    {
        if (admxDirectory == null)
            throw new ArgumentNullException(nameof(admxDirectory));
        if (!admxDirectory.Loaded)
            throw new InvalidOperationException("ADMX directory is not loaded.");
        return admxDirectory.LoadedAdmxContents.FirstOrDefault(x => string.Equals(x.TargetNamespace.@namespace, @namespace, StringComparison.Ordinal));
    }

    /// <summary>
    /// Gets the AdmxContent matching the prefix.
    /// </summary>
    /// <param name="admxDirectory">
    /// The ADMX directory to check.
    /// </param>
    /// <param name="prefix">
    /// The prefix to match.
    /// </param>
    /// <returns>
    /// AdmxContent matching the prefix.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="admxDirectory"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// ADMX directory is not loaded.
    /// </exception>
    public static AdmxContent? GetAdmxContentByPrefix(this AdmxDirectory admxDirectory, string prefix)
    {
        if (admxDirectory == null)
            throw new ArgumentNullException(nameof(admxDirectory));
        if (!admxDirectory.Loaded)
            throw new InvalidOperationException("ADMX directory is not loaded.");
        return admxDirectory.LoadedAdmxContents.FirstOrDefault(x => string.Equals(x.TargetNamespace.prefix, prefix, StringComparison.Ordinal));
    }

    /// <summary>
    /// Gets the default ADMX resource.
    /// </summary>
    /// <remarks>
    /// This method returns the first ADMX resource that is loaded. If the English (United States) resource is available, it will be returned.
    /// </remarks>
    /// <param name="admxContent">
    /// The ADMX content to get the default ADMX resource.
    /// </param>
    /// <returns>
    /// The default ADMX resource.
    /// </returns>
    public static AdmlResource? GetDefaultAdmlResource(this AdmxContent admxContent)
    {
        if (admxContent == null)
            throw new ArgumentNullException(nameof(admxContent));
        if (!admxContent.Loaded)
            throw new InvalidOperationException("ADMX content is not loaded.");
        var resources = admxContent.LoadedAdmlResources;
        var enUsAdml = resources.FirstOrDefault(x => x.Key == new CultureInfo("en-US")).Value;
        if (enUsAdml != null)
            return enUsAdml;
        else
            return resources.FirstOrDefault().Value;
    }

    /// <summary>
    /// Gets the policy presentation of the specified <see cref="PolicyDefinition"/> object.
    /// </summary>
    /// <remarks>
    /// Policy presentation is a user interface descripion of the policy elements. Presentation contains default values, list items, and other information.
    /// </remarks>
    /// <param name="policyDefinition">
    /// The policy definition to get the policy presentation.
    /// </param>
    /// <param name="admxContent">
    /// The ADMX content to get the policy presentation.
    /// </param>
    /// <returns>
    /// The policy presentation of the specified <see cref="PolicyDefinition"/> object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="policyDefinition"/> or <paramref name="admxContent"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// ADMX content is not loaded.
    /// </exception>
    public static PolicyPresentation? GetPolicyPresentation(this PolicyDefinition policyDefinition, AdmxContent admxContent)
    {
        if (policyDefinition == null)
            throw new ArgumentNullException(nameof(policyDefinition));
        if (admxContent == null)
            throw new ArgumentNullException(nameof(admxContent));
        if (!admxContent.Loaded)
            throw new InvalidOperationException("ADMX content is not loaded.");

        var @ref = policyDefinition.presentation;
        var regex = _regexFactory.Value;
        var match = regex.Match(@ref);

        if (match == null || !match.Success)
            return default;

        var resourceType = match.Groups["ResourceType"].Value;
        var resourceKey = match.Groups["ResourceKey"].Value;

        if (string.IsNullOrWhiteSpace(resourceType) || string.IsNullOrWhiteSpace(resourceKey))
            return default;

        if (!string.Equals("presentation", resourceType, StringComparison.OrdinalIgnoreCase))
            return default;

        var defaultResource = admxContent.GetDefaultAdmlResource();
        if (defaultResource == null)
            return default;

        return defaultResource.PresentationTable.FirstOrDefault(x => string.Equals(resourceKey, x.id, StringComparison.Ordinal));
    }

    private static string? GetStringProperty(object id, string propertyName)
    {
        if (id == null)
            throw new ArgumentNullException(nameof(id));

        var type = id.GetType();
        var val = type.GetProperty(propertyName);

        if (val == null || val.PropertyType != typeof(string))
            throw new ArgumentException($"Cannot find property '{propertyName}' on the object.");

        return (string?)val.GetValue(id);
    }

    private static object? GetObjectProperty(object id, string propertyName)
    {
        if (id == null)
            throw new ArgumentNullException(nameof(id));

        var type = id.GetType();
        var val = type.GetProperty(propertyName);

        if (val == null || val.PropertyType != typeof(object))
            throw new ArgumentException($"Cannot find property '{propertyName}' on the object.");

        return val.GetValue(id);
    }

    /// <summary>
    /// Gets the element IDs of the specified <see cref="PolicyDefinition"/> object.
    /// </summary>
    /// <param name="policyDefinition">
    /// The policy definition to get the element IDs.
    /// </param>
    /// <returns>
    /// The element IDs of the specified <see cref="PolicyDefinition"/> object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="policyDefinition"/> is <see langword="null"/>.
    /// </exception>
    public static IEnumerable<string> GetElementIds(this PolicyDefinition policyDefinition)
    {
        if (policyDefinition == null)
            throw new ArgumentNullException(nameof(policyDefinition));

        var list = new List<string>();

        if (policyDefinition.elements != null)
        {
            foreach (var eachElement in policyDefinition.elements)
            {
                var id = GetStringProperty(eachElement, "id");
                if (id == null)
                    continue;
                list.Add(id);
            }
        }

        return list.ToArray();
    }

    /// <summary>
    /// Gets the default value of the specified element ID of the <see cref="PolicyDefinition"/> object.
    /// </summary>
    /// <param name="policyDefinition">
    /// The policy definition to get the default value.
    /// </param>
    /// <param name="admxContent">
    /// The ADMX content to get the default value.
    /// </param>
    /// <param name="elementId">
    /// The element ID to get the default value.
    /// </param>
    /// <returns>
    /// The default value of the specified element ID of the <see cref="PolicyDefinition"/> object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="policyDefinition"/>, <paramref name="admxContent"/>, or <paramref name="elementId"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// ADMX content is not loaded.
    /// </exception>
    public static object? GetElementDefaultValue(this PolicyDefinition policyDefinition, AdmxContent admxContent, string elementId)
    {
        if (policyDefinition == null)
            throw new ArgumentNullException(nameof(policyDefinition));
        if (admxContent == null)
            throw new ArgumentNullException(nameof(admxContent));
        if (!admxContent.Loaded)
            throw new InvalidOperationException("ADMX content is not loaded.");

        var presentation = policyDefinition.GetPolicyPresentation(admxContent);

        if (presentation == null)
            return null;

        foreach (var eachItem in presentation.Items)
        {
            var refIdProperty = GetStringProperty(eachItem, "refId");

            if (!string.Equals(elementId, refIdProperty, StringComparison.Ordinal))
                continue;

            if (eachItem is not ListBox)
                return GetObjectProperty(eachItem, "defaultValue");
            else
                return new string[] { };
        }
        return null;
    }

    /// <summary>
    /// Gets the default value of the specified element ID of the <see cref="PolicyDefinition"/> object.
    /// </summary>
    /// <param name="admxContent">
    /// The ADMX content to get the default value.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="admxContent"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// ADMX content is not loaded.
    /// </exception>
    /// <returns>
    /// The default value of the specified element ID of the <see cref="PolicyDefinition"/> object.
    /// </returns>
    public static IEnumerable<PolicyDefinition> GetUserPolicies(this AdmxContent admxContent)
    {
        if (admxContent == null)
            throw new ArgumentNullException(nameof(admxContent));
        if (!admxContent.Loaded)
            throw new InvalidOperationException("ADMX content is not loaded.");
        return admxContent.Policies
            .Where(x => x.@class == PolicyClass.User || x.@class == PolicyClass.Both)
            .ToArray();
    }

    /// <summary>
    /// Gets the user policy of the specified policy name.
    /// </summary>
    /// <param name="admxContent">
    /// The ADMX content to get the user policy.
    /// </param>
    /// <param name="policyName">
    /// The policy name to get the user policy.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="admxContent"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// ADMX content is not loaded.
    /// </exception>
    /// <returns>
    /// The user policy of the specified policy name.
    /// </returns>
    public static PolicyDefinition? GetUserPolicy(this AdmxContent admxContent, string policyName)
    {
        if (admxContent == null)
            throw new ArgumentNullException(nameof(admxContent));
        if (!admxContent.Loaded)
            throw new InvalidOperationException("ADMX content is not loaded.");
        return GetUserPolicies(admxContent)
            .FirstOrDefault(x => string.Equals(policyName, x.name, StringComparison.Ordinal));
    }

    /// <summary>
    /// Gets the user policy of the specified <see cref="PolicyDefinition"/> object.
    /// </summary>
    /// <param name="policyDefinition">
    /// The policy definition to get the user policy.
    /// </param>
    /// <returns>
    /// The user policy of the specified <see cref="PolicyDefinition"/> object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="policyDefinition"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// The selected policy is not designed for user.
    /// </exception>
    public static bool? GetUserPolicy(this PolicyDefinition policyDefinition)
    {
        if (policyDefinition == null)
            throw new ArgumentNullException(nameof(policyDefinition));

        var isUserPolicy = policyDefinition.@class == PolicyClass.User || policyDefinition.@class == PolicyClass.Both;

        if (isUserPolicy == false)
            throw new NotSupportedException("Selected policy is not designed for user.");

        var key = policyDefinition.key;
        var valueName = policyDefinition.valueName;
        var query = GroupPolicy.GetUserPolicy(key, valueName);
        var hasEntry = query.KeyExists && query.ValueExists;

        if (query.Value != null)
        {
            if (policyDefinition.enabledValue != null)
            {
                if (policyDefinition.enabledValue.IsDeleteValue())
                    return hasEntry == false;
                else
                    return AdmxParser.AdmxExtensions.Equals(policyDefinition.enabledValue, ToAdmxValue(query.Value));
            }
            else if (policyDefinition.disabledValue != null)
            {
                if (policyDefinition.disabledValue.IsDeleteValue())
                    return hasEntry == false;
                else
                    return AdmxParser.AdmxExtensions.Equals(policyDefinition.disabledValue, ToAdmxValue(query.Value));
            }
        }

        return null;
    }

    /// <summary>
    /// Resets the user elements of the specified <see cref="PolicyDefinition"/> object.
    /// </summary>
    /// <param name="policyDefinition">
    /// The policy definition to reset the user elements.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="policyDefinition"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// The selected policy is not designed for user.
    /// </exception>
    public static void ResetUserElements(this PolicyDefinition policyDefinition)
    {
        if (policyDefinition == null)
            throw new ArgumentNullException(nameof(policyDefinition));

        var isUserPolicy = policyDefinition.@class == PolicyClass.User || policyDefinition.@class == PolicyClass.Both;

        if (isUserPolicy == false)
            throw new NotSupportedException("Selected policy is not designed for user.");

        var key = policyDefinition.key;
        var valueName = policyDefinition.valueName;

        if (policyDefinition.elements != null)
        {
            foreach (var eachElement in policyDefinition.elements)
            {
                var elemKey = GetStringProperty(eachElement, "key");
                elemKey = string.IsNullOrWhiteSpace(elemKey) ? key : elemKey;

                if (elemKey == null)
                    continue;

                if (eachElement is ListElement le)
                {
                    var results = GroupPolicy.GetUserPolicies(elemKey, le.valuePrefix);
                    foreach (var eachItem in results.Results)
                        GroupPolicy.DeleteUserPolicy(elemKey, eachItem.ValueName);
                }
                else
                {
                    var elemValue = GetStringProperty(eachElement, "valueName")!;
                    GroupPolicy.DeleteUserPolicy(elemKey, elemValue);
                }
            }
        }
    }

    /// <summary>
    /// Resets the user policy of the specified <see cref="PolicyDefinition"/> object.
    /// </summary>
    /// <param name="policyDefinition">
    /// The policy definition to reset the user policy.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="policyDefinition"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// The selected policy is not designed for user.
    /// </exception>
    public static void ResetUserPolicy(this PolicyDefinition policyDefinition)
    {
        if (policyDefinition == null)
            throw new ArgumentNullException(nameof(policyDefinition));

        var isUserPolicy = policyDefinition.@class == PolicyClass.User || policyDefinition.@class == PolicyClass.Both;

        if (isUserPolicy == false)
            throw new NotSupportedException("Selected policy is not designed for user.");

        var key = policyDefinition.key;
        var valueName = policyDefinition.valueName;

        // Reset
        GroupPolicy.DeleteUserPolicy(key, valueName);

        if (policyDefinition.enabledList != null)
        {
            var enableKey = policyDefinition.enabledList.defaultKey;
            enableKey = string.IsNullOrWhiteSpace(enableKey) ? key : enableKey;
            foreach (var eachEnableItem in policyDefinition.enabledList.item)
            {
                var eachEnableKey = eachEnableItem.key;
                eachEnableKey = string.IsNullOrWhiteSpace(eachEnableKey) ? enableKey : eachEnableKey;
                GroupPolicy.DeleteUserPolicy(eachEnableKey, eachEnableItem.valueName);
            }
        }

        if (policyDefinition.disabledList != null)
        {
            var disableKey = policyDefinition.disabledList.defaultKey;
            disableKey = string.IsNullOrWhiteSpace(disableKey) ? key : disableKey;
            foreach (var eachDisableItem in policyDefinition.disabledList.item)
            {
                var eachDisableKey = eachDisableItem.key;
                eachDisableKey = string.IsNullOrWhiteSpace(eachDisableKey) ? disableKey : eachDisableKey;
                GroupPolicy.DeleteUserPolicy(eachDisableKey, eachDisableItem.valueName);
            }
        }

        ResetUserElements(policyDefinition);
    }

    /// <summary>
    /// Sets the user policy of the specified <see cref="PolicyDefinition"/> object.
    /// </summary>
    /// <param name="policyDefinition">
    /// The policy definition to set the user policy.
    /// </param>
    /// <param name="enable">
    /// <see langword="true"/> to enable the policy; otherwise, <see langword="false"/>.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="policyDefinition"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// The selected policy is not designed for user.
    /// </exception>
    public static void SetUserPolicy(this PolicyDefinition policyDefinition, bool enable)
    {
        if (policyDefinition == null)
            throw new ArgumentNullException(nameof(policyDefinition));

        var isUserPolicy = policyDefinition.@class == PolicyClass.User || policyDefinition.@class == PolicyClass.Both;

        if (isUserPolicy == false)
            throw new NotSupportedException("Selected policy is not designed for user.");

        ResetUserPolicy(policyDefinition);

        var key = policyDefinition.key;
        var valueName = policyDefinition.valueName;

        if (enable)
        {
            // Enable
            if (policyDefinition.enabledValue != null)
            {
                if (policyDefinition.enabledValue.IsDeleteValue())
                    GroupPolicy.DeleteUserPolicy(key, valueName);
                else
                    GroupPolicy.SetUserPolicy(key, valueName, policyDefinition.enabledValue.GetValue(), false);
            }

            if (policyDefinition.enabledList != null)
            {
                var enableKey = policyDefinition.enabledList.defaultKey;
                enableKey = string.IsNullOrWhiteSpace(enableKey) ? key : enableKey;
                foreach (var eachEnableItem in policyDefinition.enabledList.item)
                {
                    var eachEnableKey = eachEnableItem.key;
                    eachEnableKey = string.IsNullOrWhiteSpace(eachEnableKey) ? enableKey : eachEnableKey;

                    if (eachEnableItem.value != null)
                    {
                        if (eachEnableItem.value.IsDeleteValue())
                            GroupPolicy.DeleteUserPolicy(eachEnableKey, eachEnableItem.valueName);
                        else
                            GroupPolicy.SetUserPolicy(eachEnableKey, eachEnableItem.valueName, eachEnableItem.value.GetValue(), false);
                    }
                }
            }
        }
        else
        {
            // Disable
            if (policyDefinition.disabledValue != null)
            {
                if (policyDefinition.disabledValue.IsDeleteValue())
                    GroupPolicy.DeleteUserPolicy(key, valueName);
                else
                    GroupPolicy.SetUserPolicy(key, valueName, policyDefinition.disabledValue.GetValue(), false);
            }

            if (policyDefinition.disabledList != null)
            {
                var disableKey = policyDefinition.disabledList.defaultKey;
                disableKey = string.IsNullOrWhiteSpace(disableKey) ? key : disableKey;
                foreach (var eachDisableItem in policyDefinition.disabledList.item)
                {
                    var eachDisableKey = eachDisableItem.key;
                    eachDisableKey = string.IsNullOrWhiteSpace(eachDisableKey) ? disableKey : eachDisableKey;

                    if (eachDisableItem.value != null)
                    {
                        if (eachDisableItem.value.IsDeleteValue())
                            GroupPolicy.DeleteUserPolicy(eachDisableKey, eachDisableItem.valueName);
                        else
                            GroupPolicy.SetUserPolicy(eachDisableKey, eachDisableItem.valueName, eachDisableItem.value.GetValue(), false);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Gets the user element of the specified <see cref="PolicyDefinition"/> object.
    /// </summary>
    /// <param name="policyDefinition">
    /// The policy definition to get the user element.
    /// </param>
    /// <param name="elementId">
    /// The element ID to get the user element.
    /// </param>
    /// <returns>
    /// The query result of the user element of the specified <see cref="PolicyDefinition"/> object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="policyDefinition"/> or <paramref name="elementId"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="elementId"/> is a whitespace string.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// The selected policy is not designed for user.
    /// </exception>
    public static GroupPolicyQueryResult? GetUserElement(this PolicyDefinition policyDefinition, string elementId)
    {
        if (policyDefinition == null)
            throw new ArgumentNullException(nameof(policyDefinition));
        if (string.IsNullOrWhiteSpace(elementId))
            throw new ArgumentException("Element ID cannot be null or whitespace string.", nameof(elementId));

        var isUserPolicy = policyDefinition.@class == PolicyClass.User || policyDefinition.@class == PolicyClass.Both;

        if (isUserPolicy == false)
            throw new NotSupportedException("Selected policy is not designed for user.");

        var key = policyDefinition.key;
        var valueName = policyDefinition.valueName;

        if (policyDefinition.elements != null)
        {
            foreach (var eachElement in policyDefinition.elements)
            {
                var elemKey = GetStringProperty(eachElement, "key");
                elemKey = string.IsNullOrWhiteSpace(elemKey) ? key : elemKey;

                if (elemKey == null)
                    continue;

                if (eachElement is ListElement le)
                    throw new NotSupportedException("Please use GetUserPolicies method to get list element.");
                else
                {
                    var elemValue = GetStringProperty(eachElement, "valueName")!;
                    return GroupPolicy.GetUserPolicy(elemKey, elemValue);
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Gets the user elements of the specified <see cref="PolicyDefinition"/> object.
    /// </summary>
    /// <param name="policyDefinition">
    /// The policy definition to get the user elements.
    /// </param>
    /// <param name="elementId">
    /// The element ID to get the user elements.
    /// </param>
    /// <returns>
    /// The query result of the user elements of the specified <see cref="PolicyDefinition"/> object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="policyDefinition"/> or <paramref name="elementId"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="elementId"/> is a whitespace string.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// The selected policy is not designed for user.
    /// </exception>
    public static MultipleGroupPolicyQueryResult? GetUserListElement(this PolicyDefinition policyDefinition, string elementId)
    {
        if (policyDefinition == null)
            throw new ArgumentNullException(nameof(policyDefinition));
        if (string.IsNullOrWhiteSpace(elementId))
            throw new ArgumentException("Element ID cannot be null or whitespace string.", nameof(elementId));

        var isUserPolicy = policyDefinition.@class == PolicyClass.User || policyDefinition.@class == PolicyClass.Both;

        if (isUserPolicy == false)
            throw new NotSupportedException("Selected policy is not designed for user.");

        var key = policyDefinition.key;
        var valueName = policyDefinition.valueName;

        if (policyDefinition.elements != null)
        {
            foreach (var eachElement in policyDefinition.elements)
            {
                var elemKey = GetStringProperty(eachElement, "key");
                elemKey = string.IsNullOrWhiteSpace(elemKey) ? key : elemKey;

                if (elemKey == null)
                    continue;

                if (eachElement is ListElement le)
                    return GroupPolicy.GetUserPolicies(elemKey, le.valuePrefix);
                else
                    throw new NotSupportedException("Please use GetUserPolicy method to get scalar element.");
            }
        }

        return null;
    }

    /// <summary>
    /// Resets the user element of the specified <see cref="PolicyDefinition"/> object.
    /// </summary>
    /// <param name="policyDefinition">
    /// The policy definition to reset the user element.
    /// </param>
    /// <param name="admxContent">
    /// The ADMX content to reset the user element.
    /// </param>
    /// <param name="elementId">
    /// The element ID to reset the user element.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="policyDefinition"/>, <paramref name="admxContent"/>, or <paramref name="elementId"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="elementId"/> is a whitespace string.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// ADMX content is not loaded.
    /// </exception>
    public static void ResetUserElement(this PolicyDefinition policyDefinition, AdmxContent admxContent, string elementId)
    {
        if (policyDefinition == null)
            throw new ArgumentNullException(nameof(policyDefinition));
        if (admxContent == null)
            throw new ArgumentNullException(nameof(admxContent));
        if (!admxContent.Loaded)
            throw new InvalidOperationException("ADMX content is not loaded.");
        if (string.IsNullOrWhiteSpace(elementId))
            throw new ArgumentException("Element ID cannot be null or whitespace string.", nameof(elementId));

        var isUserPolicy = policyDefinition.@class == PolicyClass.User || policyDefinition.@class == PolicyClass.Both;

        if (isUserPolicy == false)
            throw new NotSupportedException("Selected policy is not designed for user.");

        var key = policyDefinition.key;
        var valueName = policyDefinition.valueName;

        if (policyDefinition.elements != null)
        {
            foreach (var eachElement in policyDefinition.elements)
            {
                var elemId = GetStringProperty(eachElement, "id");

                if (elemId == null)
                    continue;

                var defaultValue = policyDefinition.GetElementDefaultValue(admxContent, elemId);

                var elemKey = GetStringProperty(eachElement, "key");
                elemKey = string.IsNullOrWhiteSpace(elemKey) ? key : elemKey;

                if (elemKey == null)
                    continue;

                if (eachElement is ListElement le)
                {
                    var results = GroupPolicy.GetUserPolicies(elemKey, le.valuePrefix);
                    foreach (var eachItem in results.Results)
                        GroupPolicy.DeleteUserPolicy(elemKey, eachItem.ValueName);
                }
                else
                {
                    var elemValue = GetStringProperty(eachElement, "valueName")!;

                    if (defaultValue == null)
                        GroupPolicy.DeleteUserPolicy(elemKey, elemValue);
                    else
                        GroupPolicy.SetUserPolicy(elemKey, elemValue, defaultValue, false);
                }
            }
        }
    }

    /// <summary>
    /// Sets the user element of the specified <see cref="PolicyDefinition"/> object.
    /// </summary>
    /// <param name="policyDefinition">
    /// The policy definition to set the user element.
    /// </param>
    /// <param name="elementId">
    /// The element ID to set the user element.
    /// </param>
    /// <param name="elementValue">
    /// The element value to set the user element.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="policyDefinition"/> or <paramref name="elementId"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="elementId"/> is a whitespace string.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// The selected policy is not designed for user.
    /// </exception>
    public static void SetUserElement(this PolicyDefinition policyDefinition, string elementId, object elementValue)
    {
        if (policyDefinition == null)
            throw new ArgumentNullException(nameof(policyDefinition));
        if (string.IsNullOrWhiteSpace(elementId))
            throw new ArgumentException("Element ID cannot be null or whitespace string.", nameof(elementId));

        var isUserPolicy = policyDefinition.@class == PolicyClass.User || policyDefinition.@class == PolicyClass.Both;

        if (isUserPolicy == false)
            throw new NotSupportedException("Selected policy is not designed for user.");

        var key = policyDefinition.key;
        var valueName = policyDefinition.valueName;

        if (policyDefinition.elements != null)
        {
            foreach (var eachElement in policyDefinition.elements)
            {
                var elemId = GetStringProperty(eachElement, "id");

                if (string.IsNullOrWhiteSpace(elementId))
                    continue;

                var elemKey = GetStringProperty(eachElement, "key");
                elemKey = string.IsNullOrWhiteSpace(elemKey) ? key : elemKey;

                if (elemKey == null)
                    continue;

                if (eachElement is ListElement le)
                {
                    var results = GroupPolicy.GetUserPolicies(elemKey, le.valuePrefix);
                    foreach (var eachItem in results.Results)
                        GroupPolicy.DeleteUserPolicy(elemKey, eachItem.ValueName);
                    var items = elementValue as IEnumerable<object>;
                    if (items != null)
                    {
                        var count = 1;
                        foreach (var eachItem in items)
                            GroupPolicy.SetUserPolicy(elemKey, le.valuePrefix + (count++).ToString(), eachItem, le.expandable);
                    }
                }
                else if (eachElement is TextElement te)
                {
                    var elemValue = GetStringProperty(eachElement, "valueName")!;
                    GroupPolicy.SetUserPolicy(elemKey, elemValue, elementValue, te.expandable);
                }
                else
                {
                    var elemValue = GetStringProperty(eachElement, "valueName")!;
                    GroupPolicy.SetUserPolicy(elemKey, elemValue, elementValue, false);
                }
            }
        }
    }

    /// <summary>
    /// Gets the machine policies.
    /// </summary>
    /// <param name="admxContent">
    /// The ADMX content to get the machine policies.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="admxContent"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// ADMX content is not loaded.
    /// </exception>
    /// <returns>
    /// The machine policies.
    /// </returns>
    public static IEnumerable<PolicyDefinition> GetMachinePolicies(this AdmxContent admxContent)
    {
        if (admxContent == null)
            throw new ArgumentNullException(nameof(admxContent));
        if (!admxContent.Loaded)
            throw new InvalidOperationException("ADMX content is not loaded.");
        return admxContent.Policies
            .Where(x => x.@class == PolicyClass.Machine || x.@class == PolicyClass.Both)
            .ToArray();
    }

    /// <summary>
    /// Gets the machine policy of the specified policy name.
    /// </summary>
    /// <param name="admxContent">
    /// The ADMX content to get the machine policy.
    /// </param>
    /// <param name="policyName">
    /// The policy name to get the machine policy.
    /// </param>
    /// <returns>
    /// The machine policy of the specified policy name.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="admxContent"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// ADMX content is not loaded.
    /// </exception>
    public static PolicyDefinition? GetMachinePolicy(this AdmxContent admxContent, string policyName)
    {
        if (admxContent == null)
            throw new ArgumentNullException(nameof(admxContent));
        if (!admxContent.Loaded)
            throw new InvalidOperationException("ADMX content is not loaded.");
        return GetMachinePolicies(admxContent)
            .FirstOrDefault(x => string.Equals(policyName, x.name, StringComparison.Ordinal));
    }

    /// <summary>
    /// Gets the machine policy of the specified <see cref="PolicyDefinition"/> object.
    /// </summary>
    /// <param name="policyDefinition">
    /// The policy definition to get the machine policy.
    /// </param>
    /// <returns>
    /// The machine policy of the specified <see cref="PolicyDefinition"/> object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="policyDefinition"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// The selected policy is not designed for machine.
    /// </exception>
    public static bool? GetMachinePolicy(this PolicyDefinition policyDefinition)
    {
        if (policyDefinition == null)
            throw new ArgumentNullException(nameof(policyDefinition));

        var isMachinePolicy = policyDefinition.@class == PolicyClass.Machine || policyDefinition.@class == PolicyClass.Both;

        if (isMachinePolicy == false)
            throw new NotSupportedException("Selected policy is not designed for machine.");

        var key = policyDefinition.key;
        var valueName = policyDefinition.valueName;
        var query = GroupPolicy.GetMachinePolicy(key, valueName);
        var hasEntry = query.KeyExists && query.ValueExists;

        if (query.Value != null)
        {
            if (policyDefinition.enabledValue != null)
            {
                if (policyDefinition.enabledValue.IsDeleteValue())
                    return hasEntry == false;
                else
                    return AdmxParser.AdmxExtensions.Equals(policyDefinition.enabledValue, ToAdmxValue(query.Value));
            }
            else if (policyDefinition.disabledValue != null)
            {
                if (policyDefinition.disabledValue.IsDeleteValue())
                    return hasEntry == false;
                else
                    return AdmxParser.AdmxExtensions.Equals(policyDefinition.disabledValue, ToAdmxValue(query.Value));
            }
        }

        return null;
    }

    /// <summary>
    /// Resets the machine elements of the specified <see cref="PolicyDefinition"/> object.
    /// </summary>
    /// <param name="policyDefinition">
    /// The policy definition to reset the machine elements.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="policyDefinition"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// The selected policy is not designed for machine.
    /// </exception>
    public static void ResetMachineElements(this PolicyDefinition policyDefinition)
    {
        if (policyDefinition == null)
            throw new ArgumentNullException(nameof(policyDefinition));

        var isMachinePolicy = policyDefinition.@class == PolicyClass.Machine || policyDefinition.@class == PolicyClass.Both;

        if (isMachinePolicy == false)
            throw new NotSupportedException("Selected policy is not designed for machine.");

        var key = policyDefinition.key;
        var valueName = policyDefinition.valueName;

        if (policyDefinition.elements != null)
        {
            foreach (var eachElement in policyDefinition.elements)
            {
                var elemKey = GetStringProperty(eachElement, "key");
                elemKey = string.IsNullOrWhiteSpace(elemKey) ? key : elemKey;

                if (elemKey == null)
                    continue;

                if (eachElement is ListElement le)
                {
                    var results = GroupPolicy.GetMachinePolicies(elemKey, le.valuePrefix);
                    foreach (var eachItem in results.Results)
                        GroupPolicy.DeleteMachinePolicy(elemKey, eachItem.ValueName);
                }
                else
                {
                    var elemValue = GetStringProperty(eachElement, "valueName")!;
                    GroupPolicy.DeleteMachinePolicy(elemKey, elemValue);
                }
            }
        }
    }

    /// <summary>
    /// Resets the machine policy of the specified <see cref="PolicyDefinition"/> object.
    /// </summary>
    /// <param name="policyDefinition">
    /// The policy definition to reset the machine policy.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="policyDefinition"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// The selected policy is not designed for machine.
    /// </exception>
    public static void ResetMachinePolicy(this PolicyDefinition policyDefinition)
    {
        if (policyDefinition == null)
            throw new ArgumentNullException(nameof(policyDefinition));

        var isMachinePolicy = policyDefinition.@class == PolicyClass.Machine || policyDefinition.@class == PolicyClass.Both;

        if (isMachinePolicy == false)
            throw new NotSupportedException("Selected policy is not designed for machine.");

        var key = policyDefinition.key;
        var valueName = policyDefinition.valueName;

        // Reset
        GroupPolicy.DeleteMachinePolicy(key, valueName);

        if (policyDefinition.enabledList != null)
        {
            var enableKey = policyDefinition.enabledList.defaultKey;
            enableKey = string.IsNullOrWhiteSpace(enableKey) ? key : enableKey;
            foreach (var eachEnableItem in policyDefinition.enabledList.item)
            {
                var eachEnableKey = eachEnableItem.key;
                eachEnableKey = string.IsNullOrWhiteSpace(eachEnableKey) ? enableKey : eachEnableKey;
                GroupPolicy.DeleteMachinePolicy(eachEnableKey, eachEnableItem.valueName);
            }
        }

        if (policyDefinition.disabledList != null)
        {
            var disableKey = policyDefinition.disabledList.defaultKey;
            disableKey = string.IsNullOrWhiteSpace(disableKey) ? key : disableKey;
            foreach (var eachDisableItem in policyDefinition.disabledList.item)
            {
                var eachDisableKey = eachDisableItem.key;
                eachDisableKey = string.IsNullOrWhiteSpace(eachDisableKey) ? disableKey : eachDisableKey;
                GroupPolicy.DeleteMachinePolicy(eachDisableKey, eachDisableItem.valueName);
            }
        }

        ResetMachineElements(policyDefinition);
    }

    /// <summary>
    /// Sets the machine policy of the specified <see cref="PolicyDefinition"/> object.
    /// </summary>
    /// <param name="policyDefinition">
    /// The policy definition to set the machine policy.
    /// </param>
    /// <param name="enable">
    /// <see langword="true"/> to enable the policy; otherwise, <see langword="false"/>.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="policyDefinition"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// The selected policy is not designed for machine.
    /// </exception>
    public static void SetMachinePolicy(this PolicyDefinition policyDefinition, bool enable)
    {
        if (policyDefinition == null)
            throw new ArgumentNullException(nameof(policyDefinition));

        var isMachinePolicy = policyDefinition.@class == PolicyClass.Machine || policyDefinition.@class == PolicyClass.Both;

        if (isMachinePolicy == false)
            throw new NotSupportedException("Selected policy is not designed for machine.");

        ResetMachinePolicy(policyDefinition);

        var key = policyDefinition.key;
        var valueName = policyDefinition.valueName;

        if (enable)
        {
            // Enable
            if (policyDefinition.enabledValue != null)
            {
                if (policyDefinition.enabledValue.IsDeleteValue())
                    GroupPolicy.DeleteMachinePolicy(key, valueName);
                else
                    GroupPolicy.SetMachinePolicy(key, valueName, policyDefinition.enabledValue.GetValue(), false);
            }

            if (policyDefinition.enabledList != null)
            {
                var enableKey = policyDefinition.enabledList.defaultKey;
                enableKey = string.IsNullOrWhiteSpace(enableKey) ? key : enableKey;
                foreach (var eachEnableItem in policyDefinition.enabledList.item)
                {
                    var eachEnableKey = eachEnableItem.key;
                    eachEnableKey = string.IsNullOrWhiteSpace(eachEnableKey) ? enableKey : eachEnableKey;

                    if (eachEnableItem.value != null)
                    {
                        if (eachEnableItem.value.IsDeleteValue())
                            GroupPolicy.DeleteMachinePolicy(eachEnableKey, eachEnableItem.valueName);
                        else
                            GroupPolicy.SetMachinePolicy(eachEnableKey, eachEnableItem.valueName, eachEnableItem.value.GetValue(), false);
                    }
                }
            }
        }
        else
        {
            // Disable
            if (policyDefinition.disabledValue != null)
            {
                if (policyDefinition.disabledValue.IsDeleteValue())
                    GroupPolicy.DeleteMachinePolicy(key, valueName);
                else
                    GroupPolicy.SetMachinePolicy(key, valueName, policyDefinition.disabledValue.GetValue(), false);
            }

            if (policyDefinition.disabledList != null)
            {
                var disableKey = policyDefinition.disabledList.defaultKey;
                disableKey = string.IsNullOrWhiteSpace(disableKey) ? key : disableKey;
                foreach (var eachDisableItem in policyDefinition.disabledList.item)
                {
                    var eachDisableKey = eachDisableItem.key;
                    eachDisableKey = string.IsNullOrWhiteSpace(eachDisableKey) ? disableKey : eachDisableKey;

                    if (eachDisableItem.value != null)
                    {
                        if (eachDisableItem.value.IsDeleteValue())
                            GroupPolicy.DeleteMachinePolicy(eachDisableKey, eachDisableItem.valueName);
                        else
                            GroupPolicy.SetMachinePolicy(eachDisableKey, eachDisableItem.valueName, eachDisableItem.value.GetValue(), false);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Gets the machine element of the specified <see cref="PolicyDefinition"/> object.
    /// </summary>
    /// <param name="policyDefinition">
    /// The policy definition to get the machine element.
    /// </param>
    /// <param name="elementId">
    /// The element ID to get the machine element.
    /// </param>
    /// <returns>
    /// The machine element of the specified <see cref="PolicyDefinition"/> object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="policyDefinition"/> or <paramref name="elementId"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="elementId"/> is a whitespace string.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// The selected policy is not designed for machine.
    /// </exception>
    public static object? GetMachineElement(this PolicyDefinition policyDefinition, string elementId)
    {
        if (policyDefinition == null)
            throw new ArgumentNullException(nameof(policyDefinition));
        if (string.IsNullOrWhiteSpace(elementId))
            throw new ArgumentException("Element ID cannot be null or whitespace string.", nameof(elementId));

        var isMachinePolicy = policyDefinition.@class == PolicyClass.Machine || policyDefinition.@class == PolicyClass.Both;

        if (isMachinePolicy == false)
            throw new NotSupportedException("Selected policy is not designed for machine.");

        var key = policyDefinition.key;
        var valueName = policyDefinition.valueName;

        if (policyDefinition.elements != null)
        {
            foreach (var eachElement in policyDefinition.elements)
            {
                var elemKey = GetStringProperty(eachElement, "key");
                elemKey = string.IsNullOrWhiteSpace(elemKey) ? key : elemKey;

                if (elemKey == null)
                    continue;

                if (eachElement is ListElement le)
                {
                    var items = new List<object>();
                    var results = GroupPolicy.GetMachinePolicies(elemKey, le.valuePrefix);
                    foreach (var eachItem in results.Results)
                        items.Add(GroupPolicy.GetMachinePolicy(elemKey, eachItem.ValueName));
                    return items.ToArray();
                }
                else
                {
                    var elemValue = GetStringProperty(eachElement, "valueName")!;
                    return GroupPolicy.GetMachinePolicy(elemKey, elemValue);
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Resets the machine element of the specified <see cref="PolicyDefinition"/> object.
    /// </summary>
    /// <param name="policyDefinition">
    /// The policy definition to reset the machine element.
    /// </param>
    /// <param name="admxContent">
    /// The ADMX content to reset the machine element.
    /// </param>
    /// <param name="elementId">
    /// The element ID to reset the machine element.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="policyDefinition"/>, <paramref name="admxContent"/>, or <paramref name="elementId"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// ADMX content is not loaded.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="elementId"/> is a whitespace string.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// The selected policy is not designed for machine.
    /// </exception>
    public static void ResetMachineElement(this PolicyDefinition policyDefinition, AdmxContent admxContent, string elementId)
    {
        if (policyDefinition == null)
            throw new ArgumentNullException(nameof(policyDefinition));
        if (admxContent == null)
            throw new ArgumentNullException(nameof(admxContent));
        if (!admxContent.Loaded)
            throw new InvalidOperationException("ADMX content is not loaded.");
        if (string.IsNullOrWhiteSpace(elementId))
            throw new ArgumentException("Element ID cannot be null or whitespace string.", nameof(elementId));

        var isMachinePolicy = policyDefinition.@class == PolicyClass.Machine || policyDefinition.@class == PolicyClass.Both;

        if (isMachinePolicy == false)
            throw new NotSupportedException("Selected policy is not designed for machine.");

        var key = policyDefinition.key;
        var valueName = policyDefinition.valueName;

        if (policyDefinition.elements != null)
        {
            foreach (var eachElement in policyDefinition.elements)
            {
                var elemId = GetStringProperty(eachElement, "id");

                if (elemId == null)
                    continue;

                var defaultValue = policyDefinition.GetElementDefaultValue(admxContent, elemId);

                var elemKey = GetStringProperty(eachElement, "key");
                elemKey = string.IsNullOrWhiteSpace(elemKey) ? key : elemKey;

                if (elemKey == null)
                    continue;

                if (eachElement is ListElement le)
                {
                    var results = GroupPolicy.GetMachinePolicies(elemKey, le.valuePrefix);
                    foreach (var eachItem in results.Results)
                        GroupPolicy.DeleteMachinePolicy(elemKey, eachItem.ValueName);
                }
                else
                {
                    var elemValue = GetStringProperty(eachElement, "valueName")!;

                    if (defaultValue == null)
                        GroupPolicy.DeleteMachinePolicy(elemKey, elemValue);
                    else
                        GroupPolicy.SetMachinePolicy(elemKey, elemValue, defaultValue, false);
                }
            }
        }
    }

    /// <summary>
    /// Sets the machine element of the specified <see cref="PolicyDefinition"/> object.
    /// </summary>
    /// <param name="policyDefinition">
    /// The policy definition to set the machine element.
    /// </param>
    /// <param name="elementId">
    /// The element ID to set the machine element.
    /// </param>
    /// <param name="elementValue">
    /// The element value to set the machine element.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="policyDefinition"/> or <paramref name="elementId"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="elementId"/> is a whitespace string.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// The selected policy is not designed for machine.
    /// </exception>
    public static void SetMachineElement(this PolicyDefinition policyDefinition, string elementId, object elementValue)
    {
        if (policyDefinition == null)
            throw new ArgumentNullException(nameof(policyDefinition));
        if (string.IsNullOrWhiteSpace(elementId))
            throw new ArgumentException("Element ID cannot be null or whitespace string.", nameof(elementId));

        var isMachinePolicy = policyDefinition.@class == PolicyClass.Machine || policyDefinition.@class == PolicyClass.Both;

        if (isMachinePolicy == false)
            throw new NotSupportedException("Selected policy is not designed for machine.");

        var key = policyDefinition.key;
        var valueName = policyDefinition.valueName;

        if (policyDefinition.elements != null)
        {
            foreach (var eachElement in policyDefinition.elements)
            {
                var elemId = GetStringProperty(eachElement, "id");

                if (string.IsNullOrWhiteSpace(elementId))
                    continue;

                var elemKey = GetStringProperty(eachElement, "key");
                elemKey = string.IsNullOrWhiteSpace(elemKey) ? key : elemKey;

                if (elemKey == null)
                    continue;

                if (eachElement is ListElement le)
                {
                    var results = GroupPolicy.GetMachinePolicies(elemKey, le.valuePrefix);
                    foreach (var eachItem in results.Results)
                        GroupPolicy.DeleteMachinePolicy(elemKey, eachItem.ValueName);
                    var items = elementValue as IEnumerable<object>;
                    if (items != null)
                    {
                        var count = 1;
                        foreach (var eachItem in items)
                            GroupPolicy.SetMachinePolicy(elemKey, le.valuePrefix + (count++).ToString(), eachItem, le.expandable);
                    }
                }
                else if (eachElement is TextElement te)
                {
                    var elemValue = GetStringProperty(eachElement, "valueName")!;
                    GroupPolicy.SetMachinePolicy(elemKey, elemValue, elementValue, te.expandable);
                }
                else
                {
                    var elemValue = GetStringProperty(eachElement, "valueName")!;
                    GroupPolicy.SetMachinePolicy(elemKey, elemValue, elementValue, false);
                }
            }
        }
    }
}
