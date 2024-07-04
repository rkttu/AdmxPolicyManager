using AdmxPolicyManager.Internals;
using AdmxPolicyManager.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading;

namespace AdmxPolicyManager;

/// <summary>
/// Represents a Group Policy Object and provides methods to interact with group policies.
/// </summary>
internal sealed partial class GroupPolicyObject : IDisposable
{
    public GroupPolicyObject()
    {
        _groupPolicyObject = new GroupPolicyClass();
        _groupPolicyObject2 = (IGroupPolicyObject2)_groupPolicyObject;
    }

    /// <summary>
    /// Finalizer for the GroupPolicyObject class.
    /// </summary>
    ~GroupPolicyObject()
        => Dispose(false);

    /// <summary>
    /// Disposes of the resources used by the GroupPolicyObject.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool _)
    {
        if (!_disposed)
        {
            if (_groupPolicyObject2 != null)
            {
                Marshal.ReleaseComObject(_groupPolicyObject2);
                _groupPolicyObject2 = null;
            }

            if (_groupPolicyObject != null)
            {
                Marshal.ReleaseComObject(_groupPolicyObject);
                _groupPolicyObject = null;
            }

            _disposed = true;
        }
    }

    private bool _disposed;
    private IGroupPolicyObject2? _groupPolicyObject2 = default;
    private GroupPolicyClass? _groupPolicyObject = default;

    private IGroupPolicyObject2 EnsureGroupPolicyObjectReady()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(GroupPolicyObject));

        if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
            throw new InvalidOperationException("Apartment state must be STA.");

        using (var identity = WindowsIdentity.GetCurrent())
        {
            var principal = new WindowsPrincipal(identity);
            if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
                throw new InvalidOperationException("This library must be run as an administrator.");
        }

        if (_groupPolicyObject2 == null || _groupPolicyObject == null)
            throw new InvalidOperationException("Group Policy Object is not initialized.");

        return _groupPolicyObject2;
    }

    public void OpenLocalMachine()
    {
        var gpo = EnsureGroupPolicyObjectReady();
        gpo.OpenLocalMachineGPO((int)GroupPolicyOpenFlags.LoadRegistry).AssertResultCode();
    }

    public void OpenRemoteMachine(string remoteMachineName)
    {
        var gpo = EnsureGroupPolicyObjectReady();
        gpo.OpenRemoteMachineGPO(remoteMachineName, (int)GroupPolicyOpenFlags.LoadRegistry);
    }

    public void OpenLocalUserOrGroup(string userOrGroupSid)
    {
        var gpo = EnsureGroupPolicyObjectReady();
        gpo.OpenLocalMachineGPOForPrincipal(userOrGroupSid, (int)GroupPolicyOpenFlags.LoadRegistry);
    }

    public void OpenDirectoryService(string path)
    {
        var gpo = EnsureGroupPolicyObjectReady();
        gpo.OpenDSGPO(path, (int)GroupPolicyOpenFlags.LoadRegistry);
    }

    public IntPtr GetUnsafeMachineRegistryKey()
    {
        var gpo = EnsureGroupPolicyObjectReady();
        gpo.GetRegistryKey((int)GroupPolicySection.Machine, out var hKey).AssertResultCode();
        return hKey;
    }

    public IntPtr GetUnsafeUserRegistryKey()
    {
        var gpo = EnsureGroupPolicyObjectReady();
        gpo.GetRegistryKey((int)GroupPolicySection.User, out var hKey).AssertResultCode();
        return hKey;
    }

    public void OpenGroupPolicy(GroupPolicyLocation location, string locationParameter)
    {
        switch (location)
        {
            case GroupPolicyLocation.ThisComputer:
                OpenLocalMachine();
                break;
            case GroupPolicyLocation.ThisComputersLocalUserOrGroup:
                OpenLocalUserOrGroup(locationParameter);
                break;
            case GroupPolicyLocation.RemoteComputer:
                OpenRemoteMachine(locationParameter);
                break;
            case GroupPolicyLocation.DirectoryService:
                OpenDirectoryService(locationParameter);
                break;
        }
    }

    public bool Save(bool isMachine, bool isAdd, int retryCount)
    {
        var wait = 1;
        retryCount = Math.Max(1, retryCount);
        var gpo = EnsureGroupPolicyObjectReady();

        for (var i = 0; i < retryCount; i++)
        {
            try
            {
                var result = gpo.Save(isMachine, isAdd,
                    NativeMethods.REGISTRY_EXTENSION_GUID,
                    NativeMethods.CLSID_GPESnapIn).IsSuccessCode();
                return result;
            }
            catch (FileLoadException fle)
            {
                if (unchecked((uint)fle.HResult) == NativeMethods.ERROR_SHARING_VIOLATION)
                {
                    wait += i;
                    Thread.Sleep(TimeSpan.FromSeconds(wait));
                }
            }
        }

        return false;
    }

    private GroupPolicyQueryResult GetRegistryValueCore(IntPtr hKey, string subKey, string valueName)
    {
        var size = 1;
        var data = new byte[size];

        if (NativeMethods.RegQueryValueExW(
            hKey, valueName, IntPtr.Zero,
            out var type, data, ref size).IsHasMoreDataCode())
            data = new byte[size];

        if (NativeMethods.RegQueryValueExW(
            hKey, valueName, IntPtr.Zero, out type, data, ref size).IsNotSuccessCode())
            return new GroupPolicyQueryResult() { KeyPath = subKey, ValueName = valueName, KeyExists = true, ValueExists = false, ValueType = RegistryValueType.None, Value = null, };

        switch (type)
        {
            case RegistryValueType.DWord:
                var dwordVal = (((data[0] | (data[1] << 8)) | (data[2] << 16)) | (data[3] << 24));
                return new GroupPolicyQueryResult() { KeyPath = subKey, ValueName = valueName, KeyExists = true, ValueExists = true, ValueType = type, Value = dwordVal, };

            case RegistryValueType.DWordBigEndian:
                var dwordBEVal = (((data[3] | (data[2] << 8)) | (data[1] << 16)) | (data[0] << 24));
                return new GroupPolicyQueryResult() { KeyPath = subKey, ValueName = valueName, KeyExists = true, ValueExists = true, ValueType = type, Value = dwordBEVal, };

            case RegistryValueType.QWord:
                var numLow = (uint)(((data[0] | (data[1] << 8)) | (data[2] << 16)) | (data[3] << 24));
                var numHigh = (uint)(((data[4] | (data[5] << 8)) | (data[6] << 16)) | (data[7] << 24));
                var qwordVal = (long)(((ulong)numHigh << 32) | (ulong)numLow);
                return new GroupPolicyQueryResult() { KeyPath = subKey, ValueName = valueName, KeyExists = true, ValueExists = true, ValueType = type, Value = qwordVal, };

            case RegistryValueType.MultiString:
                var strings = new List<string>();
                var packed = Encoding.Unicode.GetString(data, 0, size);
                var start = 0;
                var end = packed.IndexOf(string.Empty, start);

                while (end > start)
                {
                    strings.Add(packed.Substring(start, end - start));
                    start = end + 1;
                    end = packed.IndexOf(string.Empty, start);
                }

                var multiStringVal = strings.ToArray();
                return new GroupPolicyQueryResult() { KeyPath = subKey, ValueName = valueName, KeyExists = true, ValueExists = true, ValueType = type, Value = multiStringVal, };

            case RegistryValueType.ExpandString:
                var unexpandedString = Encoding.Unicode.GetString(data, 0, size);
                var expandableStringVal = unexpandedString.Substring(0, unexpandedString.Length - 1);
                return new GroupPolicyQueryResult() { KeyPath = subKey, ValueName = valueName, KeyExists = true, ValueExists = true, ValueType = type, Value = expandableStringVal, };

            case RegistryValueType.String:
                var rawString = Encoding.Unicode.GetString(data, 0, size);
                var stringVal = rawString.Substring(0, rawString.Length - 1);
                return new GroupPolicyQueryResult() { KeyPath = subKey, ValueName = valueName, KeyExists = true, ValueExists = true, ValueType = type, Value = stringVal, };

            default:
                return new GroupPolicyQueryResult() { KeyPath = subKey, ValueName = valueName, KeyExists = true, ValueExists = true, ValueType = type, Value = data, };
        }
    }

    public GroupPolicyQueryResult GetGroupPolicyCore(bool isMachine, string subKey, string valueName, bool useCriticalPolicySection)
    {
        var criticalSection = useCriticalPolicySection ? NativeMethods.EnterCriticalPolicySection(isMachine) : IntPtr.Zero;

        try
        {
            var gphKey = isMachine ? GetUnsafeMachineRegistryKey() : GetUnsafeUserRegistryKey();
            var hKey = default(IntPtr);

            var resultCode = NativeMethods.RegOpenKeyExW(
                gphKey, subKey, ExtendedRegistryOptions.NonVolatile,
                DesiredSAMPermissions.QueryValue, out hKey);

            if (resultCode.IsSuccessCode())
            {
                try { return GetRegistryValueCore(hKey, subKey, valueName); }
                finally { NativeMethods.RegCloseKey(hKey); }
            }

            return new GroupPolicyQueryResult() { KeyPath = subKey, ValueName = valueName, KeyExists = false, ValueExists = false, ValueType = RegistryValueType.None, Value = null, };
        }
        finally
        {
            if (criticalSection != IntPtr.Zero)
            {
                NativeMethods.LeaveCriticalPolicySection(criticalSection);
                criticalSection = IntPtr.Zero;
            }
        }
    }

    public MultipleGroupPolicyQueryResult GetGroupPoliciesCore(bool isMachine, string subKey, string? valueNamePrefix, bool useCriticalPolicySection)
    {
        var criticalSection = useCriticalPolicySection ? NativeMethods.EnterCriticalPolicySection(isMachine) : IntPtr.Zero;

        try
        {
            var list = new MultipleGroupPolicyQueryResult() { Succeed = true, };
            var gphKey = isMachine ? GetUnsafeMachineRegistryKey() : GetUnsafeUserRegistryKey();
            var hKey = default(IntPtr);

            if (NativeMethods.RegOpenKeyExW(
                gphKey, subKey, ExtendedRegistryOptions.NonVolatile,
                DesiredSAMPermissions.QueryValue, out hKey).IsSuccessCode())
            {
                try
                {
                    var index = 0;
                    var valueNameBuffer = new StringBuilder(NativeMethods.MAX_REG_VALUE_NAME_SIZE);
                    var valueNameLength = valueNameBuffer.Capacity + 1;

                    while (true)
                    {
                        var resultCode = NativeMethods.RegEnumValueW(
                            hKey, index, valueNameBuffer, ref valueNameLength,
                            IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
                        if (resultCode.IsSuccessCode())
                        {
                            var valueName = valueNameBuffer.ToString();

                            if (string.IsNullOrWhiteSpace(valueNamePrefix))
                                list.Results.Add(GetRegistryValueCore(hKey, subKey, valueName));
                            else if (valueName.StartsWith(valueNamePrefix))
                                list.Results.Add(GetRegistryValueCore(hKey, subKey, valueName));

                            index++;
                            valueNameLength = valueNameBuffer.Capacity + 1;
                            continue;
                        }
                        else if (resultCode.IsErrorNoMoreItemsCode())
                            break;
                        else
                        {
                            list.Succeed = false;
                            list.LastErrorCode = resultCode;
                            return list;
                        }
                    }
                }
                finally
                {
                    NativeMethods.RegCloseKey(hKey);
                }
            }

            return list;
        }
        finally
        {
            if (criticalSection != IntPtr.Zero)
            {
                NativeMethods.LeaveCriticalPolicySection(criticalSection);
                criticalSection = IntPtr.Zero;
            }
        }
    }

    public GroupPolicyUpdateResult SetGroupPolicyCore(bool isMachine, string subKey, string valueName, object value, bool requireExpandString, int retryCount)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        var gphKey = isMachine ? GetUnsafeMachineRegistryKey() : GetUnsafeUserRegistryKey();
        var gphSubKey = default(IntPtr);

        try
        {
            if (NativeMethods.RegCreateKeyExW(gphKey, subKey, 0, null, ExtendedRegistryOptions.NonVolatile,
                DesiredSAMPermissions.Write, IntPtr.Zero, out gphSubKey, out var flag).IsNotSuccessCode())
                return GroupPolicyUpdateResult.CreateOrOpenFailed;

            var keyValue = IntPtr.Zero;
            var hr = 0;

            switch (value)
            {
                case int i:
                    keyValue = Marshal.AllocHGlobal(Marshal.SizeOf<int>());
                    Marshal.WriteInt32(keyValue, i);
                    hr = NativeMethods.RegSetValueExW(gphSubKey, valueName, 0, RegistryValueType.DWord, keyValue, Marshal.SizeOf<int>());
                    break;

                case uint ui:
                    keyValue = Marshal.AllocHGlobal(Marshal.SizeOf<uint>());
                    Marshal.WriteInt32(keyValue, unchecked((int)ui));
                    hr = NativeMethods.RegSetValueExW(gphSubKey, valueName, 0, RegistryValueType.DWord, keyValue, Marshal.SizeOf<uint>());
                    break;

                case long l:
                    keyValue = Marshal.AllocHGlobal(Marshal.SizeOf<long>());
                    Marshal.WriteInt64(keyValue, l);
                    hr = NativeMethods.RegSetValueExW(gphSubKey, valueName, 0, RegistryValueType.DWord, keyValue, Marshal.SizeOf<long>());
                    break;

                case ulong ul:
                    keyValue = Marshal.AllocHGlobal(Marshal.SizeOf<ulong>());
                    Marshal.WriteInt64(keyValue, unchecked((long)ul));
                    hr = NativeMethods.RegSetValueExW(gphSubKey, valueName, 0, RegistryValueType.DWord, keyValue, Marshal.SizeOf<ulong>());
                    break;

                case string s:
                    keyValue = Marshal.StringToHGlobalUni(s);
                    hr = NativeMethods.RegSetValueExW(gphSubKey, valueName, 0, requireExpandString ? RegistryValueType.ExpandString : RegistryValueType.String, keyValue, s.Length * 2 + 2);
                    break;

                default:
                    return GroupPolicyUpdateResult.SetFailed;
            }

            if (hr.IsNotSuccessCode())
                return GroupPolicyUpdateResult.SetFailed;

            if (!Save(isMachine, true, retryCount))
                return GroupPolicyUpdateResult.SaveFailed;

            return GroupPolicyUpdateResult.UpdateSucceed;
        }
        finally
        {
            NativeMethods.RegCloseKey(gphSubKey);
            NativeMethods.RegCloseKey(gphKey);
        }
    }

    public GroupPolicyDeleteResult DeleteGroupPolicyCore(bool isMachine, string subKey, string valueName, int retryCount)
    {
        var gphKey = isMachine ? GetUnsafeMachineRegistryKey() : GetUnsafeUserRegistryKey();
        var hKey = default(IntPtr);

        try
        {
            if (NativeMethods.RegOpenKeyExW(
                gphKey, subKey, ExtendedRegistryOptions.NonVolatile, DesiredSAMPermissions.QueryValue, out hKey)
                .IsSuccessCode())
            {
                NativeMethods.RegCloseKey(hKey);
                hKey = IntPtr.Zero;

                if (NativeMethods.RegDeleteKeyExW(gphKey, subKey, DesiredSAMPermissions.Write, 0).IsNotSuccessCode())
                {
                    NativeMethods.RegCloseKey(gphKey);
                    return GroupPolicyDeleteResult.CreateOrOpenFailed;
                }

                if (!Save(isMachine, false, retryCount))
                    return GroupPolicyDeleteResult.SaveFailed;

                return GroupPolicyDeleteResult.DeleteSucceed;
            }
            else
                return GroupPolicyDeleteResult.NoItemFound;
        }
        finally
        {
            if (hKey != IntPtr.Zero)
                NativeMethods.RegCloseKey(hKey);

            NativeMethods.RegCloseKey(gphKey);
        }
    }
}
