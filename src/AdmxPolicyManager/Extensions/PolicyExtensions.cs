using AdmxParser;
using AdmxParser.Serialization;
using AdmxPolicyManager.Interop;
using AdmxPolicyManager.Models.Elements;
using AdmxPolicyManager.Models.Policies;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Principal;
using System.Threading;

// Please do not update the namespace for convience of usage in the consumer projects.
namespace AdmxPolicyManager
{
    /// <summary>
    /// Extension methods for the <see cref="PolicyRegistryValue"/> class.
    /// </summary>
    public static class PolicyExtensions
    {
        /// <summary>
        /// Converts the <see cref="WellKnownSidType"/> enumeration value to a string SID.
        /// </summary>
        /// <param name="wellKnownSids">
        /// The <see cref="WellKnownSidType"/> enumeration value to be converted.
        /// </param>
        /// <param name="domainSid">
        /// The domain SID to be used for the conversion. This is optional unless you are converting a well-known SID that requires a domain SID.
        /// </param>
        /// <returns>
        /// The string SID.
        /// </returns>
        public static string ConvertToSid(this WellKnownSidType wellKnownSids, SecurityIdentifier domainSid = default)
            => Helpers.WellKnownSidToStringSidInternal((int)wellKnownSids, domainSid);

        /// <summary>
        /// Converts the <see cref="ExtendedWellKnownSidType"/> enumeration value to a string SID.
        /// </summary>
        /// <remarks>
        /// Built-in <see cref="WellKnownSidType"/> lacks some latest well-known SIDs. This method provides a way to convert the extended well-known SIDs to string SID.
        /// </remarks>
        /// <param name="wellKnownSids">
        /// The <see cref="ExtendedWellKnownSidType"/> enumeration value to be converted.
        /// </param>
        /// <param name="domainSid">
        /// The domain SID to be used for the conversion. This is optional unless you are converting a well-known SID that requires a domain SID.
        /// </param>
        /// <returns>
        /// The string SID.
        /// </returns>
        public static string ConvertToSid(this ExtendedWellKnownSidType wellKnownSids, SecurityIdentifier domainSid = default)
            => Helpers.WellKnownSidToStringSidInternal((int)wellKnownSids, domainSid);

        /// <summary>
        /// Read the raw value of the specified registry value.
        /// </summary>
        /// <param name="target">
        /// The <see cref="PolicyRegistryValue"/> object that contains the registry key path and the registry value name.
        /// </param>
        /// <param name="section">
        /// The policy section where the registry key path is located.
        /// </param>
        /// <param name="userOrGroupSid">
        /// The SID of the user or group that is reading the registry value.
        /// </param>
        /// <returns>
        /// The raw value of the specified registry value.
        /// </returns>
        public static Value ReadRawValue(this PolicyRegistryValue target, PolicySection section, string userOrGroupSid)
            => ReadValue(section, userOrGroupSid, target.RegistryKeyPath, target.RegistryValueName, default);

        /// <summary>
        /// Read the raw values of the specified registry values.
        /// </summary>
        /// <param name="target">
        /// The <see cref="PolicyListElementInfo"/> object that contains the registry key path and the registry value prefix.
        /// </param>
        /// <param name="section">
        /// The policy section where the registry key path is located.
        /// </param>
        /// <param name="userOrGroupSid">
        /// The SID of the user or group that is reading the registry values.
        /// </param>
        /// <returns></returns>
        public static IReadOnlyList<KeyValuePair<string, Value>> ReadRawValues(this PolicyListElementInfo target, PolicySection section, string userOrGroupSid)
        {
            var list = new List<KeyValuePair<string, Value>>();

            foreach (var eachItem in ReadValuesWithPrefix(section, userOrGroupSid, target.RegistryKeyPath, target.RegistryValuePrefix, default))
                list.Add(eachItem);

            return list.AsReadOnly();
        }

        /// <summary>
        /// Check equality between the specified registry value and the provided <see cref="PolicyRegistryValue"/> object.
        /// </summary>
        /// <param name="value">
        /// The <see cref="PolicyRegistryValue"/> object that contains the registry key path and the registry value name.
        /// </param>
        /// <param name="prv">
        /// The <see cref="PolicyRegistryValue"/> object that contains the registry key path and the registry value name.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the registry values are equal; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool Equals(this Value value, Value prv)
        {
            if (value == null)
                return prv == null;
            if (prv == null)
                return value == null;
            if (value.Item is ValueDelete && prv.Item is ValueDelete)
                return true;

            if (value.Item is ValueDecimal vd)
            {
                if (!(prv.Item is ValueDecimal))
                    return false;

                var pvd = (ValueDecimal)prv.Item;
                return vd.value == pvd.value;
            }

            if (value.Item is ValueLongDecimal vld)
            {
                if (!(prv.Item is ValueLongDecimal))
                    return false;

                var pvld = (ValueLongDecimal)prv.Item;
                return vld.value == pvld.value;
            }

            if (value.Item is string s)
            {
                if (!(prv.Item is string))
                    return false;

                var ps = (string)prv.Item;
                return string.Equals(s, ps);
            }

            return false;
        }

        /// <summary>
        /// Check whether the specified registry value is applied.
        /// </summary>
        /// <param name="target">
        /// The <see cref="PolicyRegistryValue"/> object that contains the registry key path and the registry value name.
        /// </param>
        /// <param name="section">
        /// The policy section where the registry key path is located.
        /// </param>
        /// <param name="userOrGroupSid">
        /// The SID of the user or group that is reading the registry value.
        /// </param>
        /// <returns>
        /// The status of the registry value.
        /// </returns>
        public static PolicyValueStatus IsApplied(this PolicyRegistryValue target, PolicySection section, string userOrGroupSid)
        {
            var state = ReadValue(section, userOrGroupSid, target.RegistryKeyPath, target.RegistryValueName, default);

            if (state == null)
                return PolicyValueStatus.NotConfigured;
            else if (state.Equals(target.Value))
                return PolicyValueStatus.ConfiguredAndMatches;
            else
                return PolicyValueStatus.ConfiguredButDifferent;
        }

        /// <summary>
        /// Apply the specified registry value to the registry key path.
        /// </summary>
        /// <param name="target">
        /// The <see cref="PolicyRegistryValue"/> object that contains the registry key path and the registry value name.
        /// </param>
        /// <param name="section">
        /// The policy section where the registry key path is located.
        /// </param>
        /// <param name="subjectId">
        /// The GUID of the subject that is applying the registry value.
        /// </param>
        /// <param name="userOrGroupSid">
        /// The SID of the user or group that is applying the registry value.
        /// </param>
        /// <param name="retryCount">
        /// The number of retry count when saving the registry value.
        /// </param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static PolicyModifyResult Apply(this PolicyRegistryValue target, PolicySection section, Guid subjectId, string userOrGroupSid, int retryCount = GroupPolicy.DefaultSaveRetryCount)
        {
            var state = ReadValue(section, userOrGroupSid, target.RegistryKeyPath, target.RegistryValueName, default);

            if (state == null)
            {
                if (target.IsDeleteValue)
                    return PolicyModifyResult.NotChanged;
                else
                {
                    WriteValue(subjectId, section, userOrGroupSid, target.RegistryKeyPath, target.RegistryValueName, target.Value, default, retryCount);
                    return PolicyModifyResult.Updated;
                }
            }
            else
            {
                if (target.IsDeleteValue)
                {
                    DeleteValue(subjectId, section, userOrGroupSid, target.RegistryKeyPath, target.RegistryValueName, retryCount);
                    return PolicyModifyResult.Deleted;
                }
                else if (!state.Equals(target.Value))
                {
                    WriteValue(subjectId, section, userOrGroupSid, target.RegistryKeyPath, target.RegistryValueName, target.Value, default, retryCount);
                    return PolicyModifyResult.Updated;
                }
                else
                    return PolicyModifyResult.Skipped;
            }
        }

        /// <summary>
        /// Reset the specified registry value to the default value.
        /// </summary>
        /// <param name="target">
        /// The <see cref="PolicyRegistryValue"/> object that contains the registry key path and the registry value name.
        /// </param>
        /// <param name="section">
        /// The policy section where the registry key path is located.
        /// </param>
        /// <param name="subjectId">
        /// The GUID of the subject that is resetting the registry value.
        /// </param>
        /// <param name="userOrGroupSid">
        /// The SID of the user or group that is resetting the registry value.
        /// </param>
        /// <param name="retryCount">
        /// The number of retry count when saving the registry value.
        /// </param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static PolicyModifyResult Reset(this PolicyRegistryValue target, PolicySection section, Guid subjectId, string userOrGroupSid, int retryCount = GroupPolicy.DefaultSaveRetryCount)
        {
            var state = ReadValue(section, userOrGroupSid, target.RegistryKeyPath, target.RegistryValueName, default);

            if (state == null)
                return PolicyModifyResult.NotChanged;

            DeleteValue(subjectId, section, userOrGroupSid, target.RegistryKeyPath, target.RegistryValueName, retryCount);
            return PolicyModifyResult.Deleted;
        }

        /// <summary>
        /// Read the multiple raw values of the specified registry value with prefix.
        /// </summary>
        /// <param name="section">
        /// The policy section where the registry key path is located.
        /// </param>
        /// <param name="userOrGroupSid">
        /// The SID of the user or group that is reading the registry values.
        /// </param>
        /// <param name="registryKeyPath">
        /// The registry key path where the registry values are read.
        /// </param>
        /// <param name="registryValuePrefix">
        /// The prefix of the registry values to be read.
        /// </param>
        /// <param name="valueOptions">
        /// The registry value options.
        /// </param>
        /// <returns>
        /// The list of the registry values with the specified prefix.
        /// </returns>
        /// <exception cref="GroupPolicyManagementException">
        /// The method requires administrator privilege.
        /// </exception>
        public static IReadOnlyList<KeyValuePair<string, Value>> ReadValuesWithPrefix(
            PolicySection section, string userOrGroupSid, string registryKeyPath, string registryValuePrefix, RegistryValueOptions valueOptions = default)
        {
            if (!GroupPolicy.IsCurrentUserAdministrator())
                throw new GroupPolicyManagementException("This method requires administrator privilege.");

            if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
                throw new GroupPolicyManagementException("This method requires STA thread.");

            var list = new List<KeyValuePair<string, Value>>();

            if (registryValuePrefix == null)
                return list.AsReadOnly();

            using (var wrapper = new GroupPolicyObject())
            {
                var gpo = wrapper.GPO;
                var resultCode = NativeMethods.S_OK;

                if (userOrGroupSid != null)
                {
                    resultCode = gpo.OpenLocalMachineGPOForPrincipal(userOrGroupSid.AssertValidSid(), (int)GroupPolicyOpenOptions.LoadRegistry);
                    if (resultCode != NativeMethods.S_OK)
                        throw new GroupPolicyManagementException("Cannot open the local machine GPO.", new Win32Exception(resultCode));
                }
                else
                {
                    resultCode = gpo.OpenLocalMachineGPO((int)GroupPolicyOpenOptions.LoadRegistry);
                    if (resultCode != NativeMethods.S_OK)
                        throw new GroupPolicyManagementException("Cannot open the local machine GPO.", new Win32Exception(resultCode));
                }

                resultCode = gpo.GetRegistryKey((int)section, out IntPtr key);
                if (resultCode != NativeMethods.S_OK)
                    throw new GroupPolicyManagementException($"Cannot open the '{section}' registry key.", new Win32Exception(resultCode));

                using (var baseKey = RegistryKey.FromHandle(new SafeRegistryHandle(key, true)))
                {
                    using (var subKey = baseKey.OpenSubKey(registryKeyPath, false))
                    {
                        if (subKey == null)
                            return list.AsReadOnly();

                        foreach (var eachKey in subKey.GetValueNames().Where(x => x.StartsWith(registryValuePrefix, StringComparison.OrdinalIgnoreCase)))
                            list.Add(new KeyValuePair<string, Value>(eachKey, subKey.GetAdmxValue(eachKey, false)));
                    }
                }

                return list.AsReadOnly();
            }
        }

        /// <summary>
        /// Read the raw value of the specified registry value.
        /// </summary>
        /// <param name="section">
        /// The policy section where the registry key path is located.
        /// </param>
        /// <param name="userOrGroupSid">
        /// The SID of the user or group that is reading the registry value.
        /// </param>
        /// <param name="registryKeyPath">
        /// The registry key path where the registry value is read.
        /// </param>
        /// <param name="registryValueName">
        /// The name of the registry value.
        /// </param>
        /// <param name="valueOptions">
        /// The registry value options.
        /// </param>
        /// <returns>
        /// The raw value of the specified registry value.
        /// </returns>
        /// <exception cref="GroupPolicyManagementException">
        /// The method requires administrator privilege.
        /// </exception>
        public static Value ReadValue(
            PolicySection section, string userOrGroupSid, string registryKeyPath, string registryValueName, RegistryValueOptions valueOptions = default)
        {
            if (!GroupPolicy.IsCurrentUserAdministrator())
                throw new GroupPolicyManagementException("This method requires administrator privilege.");

            if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
                throw new GroupPolicyManagementException("This method requires STA thread.");

            using (var wrapper = new GroupPolicyObject())
            {
                var gpo = wrapper.GPO;
                var resultCode = NativeMethods.S_OK;

                if (userOrGroupSid != null)
                {
                    resultCode = gpo.OpenLocalMachineGPOForPrincipal(userOrGroupSid.AssertValidSid(), (int)GroupPolicyOpenOptions.LoadRegistry);
                    if (resultCode != NativeMethods.S_OK)
                        throw new GroupPolicyManagementException("Cannot open the local machine GPO.", new Win32Exception(resultCode));
                }
                else
                {
                    resultCode = gpo.OpenLocalMachineGPO((int)GroupPolicyOpenOptions.LoadRegistry);
                    if (resultCode != NativeMethods.S_OK)
                        throw new GroupPolicyManagementException("Cannot open the local machine GPO.", new Win32Exception(resultCode));
                }

                resultCode = gpo.GetRegistryKey((int)section, out IntPtr key);
                if (resultCode != NativeMethods.S_OK)
                    throw new GroupPolicyManagementException($"Cannot open the '{section}' registry key.", new Win32Exception(resultCode));

                using (var baseKey = RegistryKey.FromHandle(new SafeRegistryHandle(key, true)))
                {
                    using (var subKey = baseKey.OpenSubKey(registryKeyPath, false))
                    {
                        if (subKey == null)
                            return null;

                        return subKey.GetAdmxValue(registryValueName, false);
                    }
                }
            }
        }

        /// <summary>
        /// Writes a registry value into the specified registry key path.
        /// </summary>
        /// <param name="subjectGuid">
        /// The GUID of the subject that is writing the registry value.
        /// </param>
        /// <param name="section">
        /// The policy section where the registry key path is located.
        /// </param>
        /// <param name="userOrGroupSid">
        /// The SID of the user or group that is writing the registry value.
        /// </param>
        /// <param name="registryKeyPath">
        /// The registry key path where the registry value is written.
        /// </param>
        /// <param name="registryValueName">
        /// The name of the registry value.
        /// </param>
        /// <param name="value">
        /// The value to be written.
        /// </param>
        /// <param name="valueKind">
        /// The registry value kind.
        /// </param>
        /// <param name="retryCount">
        /// The number of retry count when saving the registry value.
        /// </param>
        /// <exception cref="GroupPolicyManagementException">
        /// The method requires administrator privilege.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The value object is not supported.
        /// </exception>
        public static void WriteValue(
            Guid subjectGuid, PolicySection section, string userOrGroupSid, string registryKeyPath, string registryValueName,
            Value value, RegistryValueKind valueKind = default, int retryCount = GroupPolicy.DefaultSaveRetryCount)
        {
            if (!GroupPolicy.IsCurrentUserAdministrator())
                throw new GroupPolicyManagementException("This method requires administrator privilege.");

            if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
                throw new GroupPolicyManagementException("This method requires STA thread.");

            using (var wrapper = new GroupPolicyObject())
            {
                var gpo = wrapper.GPO;
                var resultCode = NativeMethods.S_OK;

                if (userOrGroupSid != null)
                {
                    resultCode = gpo.OpenLocalMachineGPOForPrincipal(userOrGroupSid.AssertValidSid(), (int)GroupPolicyOpenOptions.LoadRegistry);
                    if (resultCode != NativeMethods.S_OK)
                        throw new GroupPolicyManagementException($"Cannot open the local GPO for principal (SID: {userOrGroupSid}).", new Win32Exception(resultCode));
                }
                else
                {
                    resultCode = gpo.OpenLocalMachineGPO((int)GroupPolicyOpenOptions.LoadRegistry);
                    if (resultCode != NativeMethods.S_OK)
                        throw new GroupPolicyManagementException("Cannot open the local GPO.", new Win32Exception(resultCode));
                }

                resultCode = gpo.GetRegistryKey((int)section, out IntPtr key);
                if (resultCode != NativeMethods.S_OK)
                    throw new GroupPolicyManagementException($"Cannot open the '{section}' registry key.", new Win32Exception(resultCode));

                using (var baseKey = RegistryKey.FromHandle(new SafeRegistryHandle(key, true)))
                {
                    using (var subKey = baseKey.CreateSubKey(registryKeyPath, true))
                    {
                        if (value.Item is ValueDecimal vd)
                            subKey.SetValue(registryValueName, (int)vd.value);
                        else if (value.Item is ValueLongDecimal vld)
                            subKey.SetValue(registryValueName, (long)vld.value);
                        else if (value.Item is string s)
                            subKey.SetValue(registryValueName, s);
                        else
                            throw new ArgumentException("Unsupported value object specified.", nameof(value));
                    }
                }

                if (userOrGroupSid == null)
                {
                    resultCode = gpo.SaveWithRetry(true, true, NativeMethods.REGISTRY_EXTENSION_GUID, subjectGuid, retryCount);
                    if (resultCode != NativeMethods.S_OK)
                        throw new GroupPolicyManagementException("Cannot save machine registry values into the local GPO.", new Win32Exception(resultCode));
                }

                resultCode = gpo.SaveWithRetry(false, true, NativeMethods.REGISTRY_EXTENSION_GUID, subjectGuid, retryCount);
                if (resultCode != NativeMethods.S_OK)
                    throw new GroupPolicyManagementException("Cannot save user registry values into the local GPO.", new Win32Exception(resultCode));
            }
        }

        /// <summary>
        /// Deletes the specified registry value.
        /// </summary>
        /// <param name="subjectGuid">
        /// The GUID of the subject that is deleting the registry value.
        /// </param>
        /// <param name="section">
        /// The policy section where the registry key path is located.
        /// </param>
        /// <param name="userOrGroupSid">
        /// The SID of the user or group that is deleting the registry value.
        /// </param>
        /// <param name="registryKeyPath">
        /// The registry key path where the registry value is deleted.
        /// </param>
        /// <param name="registryValueName">
        /// The name of the registry value to be deleted. If this is <see langword="null"/>, the default value is deleted.
        /// </param>
        /// <param name="retryCount">
        /// The number of retry count when saving the registry value.
        /// </param>
        /// <exception cref="GroupPolicyManagementException">
        /// The method requires administrator privilege.
        /// </exception>
        public static void DeleteValue(
            Guid subjectGuid, PolicySection section, string userOrGroupSid, string registryKeyPath, string registryValueName,
            int retryCount = GroupPolicy.DefaultSaveRetryCount)
        {
            if (!GroupPolicy.IsCurrentUserAdministrator())
                throw new GroupPolicyManagementException("This method requires administrator privilege.");

            if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
                throw new GroupPolicyManagementException("This method requires STA thread.");

            using (var wrapper = new GroupPolicyObject())
            {
                var gpo = wrapper.GPO;
                var resultCode = NativeMethods.S_OK;

                if (userOrGroupSid != null)
                {
                    resultCode = gpo.OpenLocalMachineGPOForPrincipal(userOrGroupSid.AssertValidSid(), (int)GroupPolicyOpenOptions.LoadRegistry);
                    if (resultCode != NativeMethods.S_OK)
                        throw new GroupPolicyManagementException($"Cannot open the local GPO for principal (SID: {userOrGroupSid}).", new Win32Exception(resultCode));
                }
                else
                {
                    resultCode = gpo.OpenLocalMachineGPO((int)GroupPolicyOpenOptions.LoadRegistry);
                    if (resultCode != NativeMethods.S_OK)
                        throw new GroupPolicyManagementException("Cannot open the local GPO.", new Win32Exception(resultCode));
                }

                resultCode = gpo.GetRegistryKey((int)section, out IntPtr key);
                if (resultCode != NativeMethods.S_OK)
                    throw new GroupPolicyManagementException($"Cannot open the '{section}' registry key.", new Win32Exception(resultCode));

                using (var baseKey = RegistryKey.FromHandle(new SafeRegistryHandle(key, true)))
                {
                    using (var subKey = baseKey.OpenSubKey(registryKeyPath, true))
                    {
                        if (subKey == null)
                            return;

                        subKey.DeleteValue(registryValueName, false);
                    }
                }

                if (userOrGroupSid == null)
                {
                    resultCode = gpo.SaveWithRetry(true, true, NativeMethods.REGISTRY_EXTENSION_GUID, subjectGuid, retryCount);
                    if (resultCode != NativeMethods.S_OK)
                        throw new GroupPolicyManagementException("Cannot save machine registry values into the local GPO.", new Win32Exception(resultCode));
                }

                resultCode = gpo.SaveWithRetry(false, true, NativeMethods.REGISTRY_EXTENSION_GUID, subjectGuid, retryCount);
                if (resultCode != NativeMethods.S_OK)
                    throw new GroupPolicyManagementException("Cannot save user registry values into the local GPO.", new Win32Exception(resultCode));
            }
        }
    }
}
