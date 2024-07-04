using AdmxParser;
using AdmxParser.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace AdmxPolicyManager;

internal static class AdmxExtensions
{
    private static readonly Lazy<Regex> _regexFactory = new Lazy<Regex>(
        () => new Regex(@"\$\((?<ResourceType>[^.]+)\.(?<ResourceKey>[^\)]+)\)", RegexOptions.Compiled | RegexOptions.IgnoreCase),
        LazyThreadSafetyMode.None);

    public static Value ToAdmxValue(this int i) => new Value() { Item = new ValueDecimal() { value = unchecked((uint)i), }, };
    public static Value ToAdmxValue(this uint ui) => new Value() { Item = new ValueDecimal() { value = ui, }, };
    public static Value ToAdmxValue(this long l) => new Value() { Item = new ValueLongDecimal() { value = unchecked((ulong)l), }, };
    public static Value ToAdmxValue(this ulong ul) => new Value() { Item = new ValueLongDecimal() { value = ul, }, };
    public static Value ToAdxmValue(this string s) => new Value() { Item = s, };
    public static Value ToAdmxValue(this object? o)
    {
        if (o is int i) return i.ToAdmxValue();
        if (o is uint ui) return ui.ToAdmxValue();
        if (o is long l) return l.ToAdmxValue();
        if (o is ulong ul) return ul.ToAdmxValue();
        if (o is string s) return s.ToAdmxValue();
        throw new NotSupportedException($"Selected type '{o?.GetType()?.Name ?? "(null)"}' is not supported.");
    }

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

    public static AdmlResource? GetDefaultAdmlResource(this AdmxContent admxContent)
    {
        var resources = admxContent.LoadedAdmlResources;
        var enUsAdml = resources.FirstOrDefault(x => x.Key == new CultureInfo("en-US")).Value;
        if (enUsAdml != null)
            return enUsAdml;
        else
            return resources.FirstOrDefault().Value;
    }

    public static PolicyPresentation? GetPolicyPresentation(this PolicyDefinition policyDefinition, AdmxContent admxContent)
    {
        if (policyDefinition == null)
            throw new ArgumentNullException(nameof(policyDefinition));

        if (admxContent == null)
            throw new ArgumentNullException(nameof(admxContent));

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

    public static object? GetElementDefaultValue(this PolicyDefinition policyDefinition, AdmxContent admxContent, string elementId)
    {
        if (policyDefinition == null)
            throw new ArgumentNullException(nameof(policyDefinition));

        if (admxContent == null)
            throw new ArgumentNullException(nameof(admxContent));

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

    public static IEnumerable<PolicyDefinition> GetUserPolicies(this AdmxContent admxContent)
    {
        return admxContent.Policies
            .Where(x => x.@class == PolicyClass.User || x.@class == PolicyClass.Both)
            .ToArray();
    }

    public static PolicyDefinition? GetUserPolicy(this AdmxContent admxContent, string policyName)
    {
        return GetUserPolicies(admxContent)
            .FirstOrDefault(x => string.Equals(policyName, x.name, StringComparison.Ordinal));
    }

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

    public static object? GetUserElement(this PolicyDefinition policyDefinition, string elementId)
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
                {
                    var items = new List<object>();
                    var results = GroupPolicy.GetUserPolicies(elemKey, le.valuePrefix);
                    foreach (var eachItem in results.Results)
                        items.Add(GroupPolicy.GetUserPolicy(elemKey, eachItem.ValueName));
                    return items.ToArray();
                }
                else
                {
                    var elemValue = GetStringProperty(eachElement, "valueName")!;
                    return GroupPolicy.GetUserPolicy(elemKey, elemValue);
                }
            }
        }

        return null;
    }

    public static void ResetUserElement(this PolicyDefinition policyDefinition, AdmxContent admxContent, string elementId)
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

    //

    public static IEnumerable<PolicyDefinition> GetMachinePolicies(this AdmxContent admxContent)
    {
        return admxContent.Policies
            .Where(x => x.@class == PolicyClass.Machine || x.@class == PolicyClass.Both)
            .ToArray();
    }

    public static PolicyDefinition? GetMachinePolicy(this AdmxContent admxContent, string policyName)
    {
        return GetMachinePolicies(admxContent)
            .FirstOrDefault(x => string.Equals(policyName, x.name, StringComparison.Ordinal));
    }

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

    public static void ResetMachineElement(this PolicyDefinition policyDefinition, AdmxContent admxContent, string elementId)
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
