using AdmxPolicyManager.Internals;
using AdmxPolicyManager.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AdmxPolicyManager;

/// <summary>
/// Contains methods for managing group policies.
/// </summary>
public static partial class GroupPolicy
{
    /// <summary>
    /// The default number of retry attempts for operations.
    /// </summary>
    public const int DefaultRetryCount = 5;
}

// Thread Procedures
partial class GroupPolicy
{
    private static void GetGroupPolicyThreadProc(object? parameter)
    {
        var context = parameter as GetGroupPolicyThreadProcContext;
        if (context == null)
            throw new ArgumentException($"Incompatible parameter type: '{parameter?.GetType()?.Name ?? "(null)"}'");
        using var o = new GroupPolicyObject();
        o.OpenGroupPolicy(context.Location, context.LocationParameter);
        context.Result = o.GetGroupPolicyCore(context.IsMachine, context.SubKey, context.ValueName, context.UseCriticalPolicySection);
    }

    private static void GetGroupPoliciesThreadProc(object? parameter)
    {
        var context = parameter as GetGroupPoliciesThreadProcContext;
        if (context == null)
            throw new ArgumentException($"Incompatible parameter type: '{parameter?.GetType()?.Name ?? "(null)"}'");
        using var o = new GroupPolicyObject();
        o.OpenGroupPolicy(context.Location, context.LocationParameter);
        context.Result = o.GetGroupPoliciesCore(context.IsMachine, context.SubKey, context.ValueNamePrefix, context.UseCriticalPolicySection);
    }

    private static void SetGroupPolicyThreadProc(object? parameter)
    {
        var context = parameter as SetGroupPolicyThreadProcContext;
        if (context == null)
            throw new ArgumentException($"Incompatible parameter type: '{parameter?.GetType()?.Name ?? "(null)"}'");
        using var o = new GroupPolicyObject();
        o.OpenGroupPolicy(context.Location, context.LocationParameter);
        context.Result = o.SetGroupPolicyCore(context.IsMachine, context.SubKey, context.ValueName, context.Value, context.RequireExpandString, context.RetryCount);
    }

    private static void DeleteGroupPolicyThreadProc(object? parameter)
    {
        var context = parameter as DeleteGroupPolicyThreadProcContext;
        if (context == null)
            throw new ArgumentException($"Incompatible parameter type: '{parameter?.GetType()?.Name ?? "(null)"}'");
        using var o = new GroupPolicyObject();
        o.OpenGroupPolicy(context.Location, context.LocationParameter);
        context.Result = o.DeleteGroupPolicyCore(context.IsMachine, context.SubKey, context.ValueName, context.RetryCount);
    }

    private static void SetMultipleGroupPolicyThreadProc(object? parameter)
    {
        var context = parameter as SetMultipleGroupPolicyThreadProcContext;
        if (context == null)
            throw new ArgumentException($"Incompatible parameter type: '{parameter?.GetType()?.Name ?? "(null)"}'");
        using var o = new GroupPolicyObject();
        o.OpenGroupPolicy(context.Location, context.LocationParameter);
        var results = new List<GroupPolicyUpdateResult>();
        foreach (var eachContext in context.Requests)
            results.Add(o.SetGroupPolicyCore(context.IsMachine, eachContext.SubKey, eachContext.ValueName, eachContext.Value, eachContext.RequireExpandString, context.RetryCount));
        context.Result = results.ToArray();
    }

    private static void DeleteMultipleGroupPolicyThreadProc(object? parameter)
    {
        var context = parameter as DeleteMultipleGroupPolicyThreadProcContext;
        if (context == null)
            throw new ArgumentException($"Incompatible parameter type: '{parameter?.GetType()?.Name ?? "(null)"}'");
        using var o = new GroupPolicyObject();
        o.OpenGroupPolicy(context.Location, context.LocationParameter);
        var results = new List<GroupPolicyDeleteResult>();
        foreach (var eachContext in context.Requests)
            results.Add(o.DeleteGroupPolicyCore(context.IsMachine, eachContext.SubKey, eachContext.ValueName, context.RetryCount));
        context.Result = results.ToArray();
    }
}

// Thread Issuers
partial class GroupPolicy
{
    private static GroupPolicyQueryResult GetGroupPolicyInternal(
        GroupPolicyLocation location, string locationParameter,
        bool isMachine, string subKey, string valueName, bool useCriticalPolicySection)
    {
        var thread = new Thread(GetGroupPolicyThreadProc);
        if (!thread.TrySetApartmentState(ApartmentState.STA))
            throw new NotSupportedException("Cannot set the new thread apartment model as STA.");
        var context = new GetGroupPolicyThreadProcContext { Location = location, LocationParameter = locationParameter, IsMachine = isMachine, SubKey = subKey, ValueName = valueName, UseCriticalPolicySection = useCriticalPolicySection, };
        thread.Start(context);
        thread.Join();
        return context.Result;
    }

    private static MultipleGroupPolicyQueryResult GetGroupPoliciesInternal(
        GroupPolicyLocation location, string locationParameter,
        bool isMachine, string subKey, string valueNamePrefix, bool useCriticalPolicySection)
    {
        var thread = new Thread(GetGroupPoliciesThreadProc);
        if (!thread.TrySetApartmentState(ApartmentState.STA))
            throw new NotSupportedException("Cannot set the new thread apartment model as STA.");
        var context = new GetGroupPoliciesThreadProcContext { Location = location, LocationParameter = locationParameter, IsMachine = isMachine, SubKey = subKey, ValueNamePrefix = valueNamePrefix, UseCriticalPolicySection = useCriticalPolicySection, };
        thread.Start(context);
        thread.Join();
        return context.Result;
    }

    private static GroupPolicyUpdateResult SetGroupPolicyInternal(
        GroupPolicyLocation location, string locationParameter,
        bool isMachine, string subKey, string valueName, object value, bool requireExpandString, int retryCount)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));
        var thread = new Thread(SetGroupPolicyThreadProc);
        if (!thread.TrySetApartmentState(ApartmentState.STA))
            throw new NotSupportedException("Cannot set the new thread apartment model as STA.");
        var context = new SetGroupPolicyThreadProcContext { Location = location, LocationParameter = locationParameter, IsMachine = isMachine, SubKey = subKey, ValueName = valueName, Value = value, RequireExpandString = requireExpandString, RetryCount = retryCount, };
        thread.Start(context);
        thread.Join();
        return context.Result;
    }

    private static GroupPolicyDeleteResult DeleteGroupPolicyInternal(
        GroupPolicyLocation location, string locationParameter,
        bool isMachine, string subKey, string valueName, int retryCount)
    {
        var thread = new Thread(DeleteGroupPolicyThreadProc);
        if (!thread.TrySetApartmentState(ApartmentState.STA))
            throw new NotSupportedException("Cannot set the new thread apartment model as STA.");
        var context = new DeleteGroupPolicyThreadProcContext { Location = location, LocationParameter = locationParameter, IsMachine = isMachine, SubKey = subKey, ValueName = valueName, RetryCount = retryCount, };
        thread.Start(context);
        thread.Join();
        return context.Result;
    }

    private static IEnumerable<GroupPolicyUpdateResult> SetMultipleGroupPolicyInternal(
        GroupPolicyLocation location, string locationParameter, bool isMachine,
        IEnumerable<SetMultipleGroupPolicyRequest> requests,
        int retryCount)
    {
        if (requests == null)
            throw new ArgumentNullException(nameof(requests));
        var thread = new Thread(SetMultipleGroupPolicyThreadProc);
        if (!thread.TrySetApartmentState(ApartmentState.STA))
            throw new NotSupportedException("Cannot set the new thread apartment model as STA.");
        var context = new SetMultipleGroupPolicyThreadProcContext
        {
            Location = location,
            LocationParameter = locationParameter,
            IsMachine = isMachine,
            Requests = requests,
            RetryCount = retryCount,
        };
        thread.Start(context);
        thread.Join();
        return context.Result;
    }

    private static IEnumerable<GroupPolicyDeleteResult> DeleteMultipleGroupPolicyInternal(
        GroupPolicyLocation location, string locationParameter, bool isMachine,
        IEnumerable<DeleteMultipleGroupPolicyRequest> requests,
        int retryCount)
    {
        if (requests == null)
            throw new ArgumentNullException(nameof(requests));
        var thread = new Thread(DeleteMultipleGroupPolicyThreadProc);
        if (!thread.TrySetApartmentState(ApartmentState.STA))
            throw new NotSupportedException("Cannot set the new thread apartment model as STA.");
        var context = new DeleteMultipleGroupPolicyThreadProcContext
        {
            Location = location,
            LocationParameter = locationParameter,
            IsMachine = isMachine,
            Requests = requests,
            RetryCount = retryCount,
        };
        thread.Start(context);
        thread.Join();
        return context.Result;
    }
}

// Policy Propagation
partial class GroupPolicy
{
    /// <summary>
    /// Propagates the machine policy.
    /// </summary>
    /// <param name="force">Indicates whether to force the policy propagation.</param>
    /// <returns>True if the policy propagation is successful, otherwise false.</returns>
    public static bool PropagateMachinePolicy(bool force = false)
    {
        return force ?
            NativeMethods.RefreshPolicyEx(true, NativeMethods.RP_FORCE) :
            NativeMethods.RefreshPolicy(true);
    }

    /// <summary>
    /// Propagates the user policy.
    /// </summary>
    /// <param name="force">Indicates whether to force the policy propagation.</param>
    /// <returns>True if the policy propagation is successful, otherwise false.</returns>
    public static bool PropagateUserPolicy(bool force = false)
    {
        return force ?
            NativeMethods.RefreshPolicyEx(false, NativeMethods.RP_FORCE) :
            NativeMethods.RefreshPolicy(false);
    }
}

// Machine Policy (for This Computer) 
partial class GroupPolicy
{
    /// <summary>
    /// Gets the machine policy value from the specified subkey and value name.
    /// </summary>
    /// <param name="subKey">The subkey to retrieve the value from.</param>
    /// <param name="valueName">The name of the value to retrieve.</param>
    /// <param name="useCriticalPolicySection">Specifies whether to use the critical policy section.</param>
    /// <returns>The <see cref="GroupPolicyQueryResult"/> containing the result of the query.</returns>
    public static GroupPolicyQueryResult GetMachinePolicy(string subKey, string valueName, bool useCriticalPolicySection = false)
        => GetGroupPolicyInternal(GroupPolicyLocation.ThisComputer, string.Empty, /* isMachine */ true, subKey, valueName, useCriticalPolicySection);

    /// <summary>
    /// Retrieves multiple group policy values for the machine.
    /// </summary>
    /// <param name="subKey">The subkey of the group policy.</param>
    /// <param name="valueNamePrefix">The prefix of the value names to retrieve.</param>
    /// <param name="useCriticalPolicySection">Specifies whether to use the critical policy section.</param>
    /// <returns>The result of the group policy query.</returns>
    public static MultipleGroupPolicyQueryResult GetMachinePolicies(string subKey, string valueNamePrefix, bool useCriticalPolicySection = false)
        => GetGroupPoliciesInternal(GroupPolicyLocation.ThisComputer, string.Empty, /* isMachine */ true, subKey, valueNamePrefix, useCriticalPolicySection);

    /// <summary>
    /// Sets the machine policy value for the specified subkey and value name.
    /// </summary>
    /// <param name="subKey">The subkey to set the value for.</param>
    /// <param name="valueName">The name of the value to set.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="requireExpandString">Specifies whether require expand string.</param>
    /// <param name="retryCount">The number of retry attempts for the operation. Default is 5.</param>
    /// <returns>The <see cref="GroupPolicyUpdateResult"/> indicating the result of the operation.</returns>
    public static GroupPolicyUpdateResult SetMachinePolicy(string subKey, string valueName, object value, bool requireExpandString, int retryCount = DefaultRetryCount)
        => SetGroupPolicyInternal(GroupPolicyLocation.ThisComputer, string.Empty, /* isMachine */ true, subKey, valueName, value, requireExpandString, retryCount);

    /// <summary>
    /// Deletes the machine policy value for the specified subkey and value name.
    /// </summary>
    /// <param name="subKey">The subkey to delete the value from.</param>
    /// <param name="valueName">The name of the value to delete.</param>
    /// <param name="retryCount">The number of retry attempts for the operation. Default is 5.</param>
    /// <returns>The <see cref="GroupPolicyDeleteResult"/> indicating the result of the operation.</returns>
    public static GroupPolicyDeleteResult DeleteMachinePolicy(string subKey, string valueName, int retryCount = DefaultRetryCount)
        => DeleteGroupPolicyInternal(GroupPolicyLocation.ThisComputer, string.Empty, /* isMachine */ true, subKey, valueName, retryCount);

    /// <summary>
    /// Sets multiple machine policies.
    /// </summary>
    /// <param name="requests">
    /// The requests to set multiple machine policies.
    /// </param>
    /// <param name="retryCount">
    /// The number of retry attempts for the operation. Default is 5.
    /// </param>
    /// <returns>
    /// The results of the operation.
    /// </returns>
    public static IEnumerable<GroupPolicyUpdateResult> SetMultipleMachinePolicies(IEnumerable<SetMultipleGroupPolicyRequest> requests, int retryCount = DefaultRetryCount)
        => SetMultipleGroupPolicyInternal(GroupPolicyLocation.ThisComputer, string.Empty, /* isMachine */ true, requests, retryCount);

    /// <summary>
    /// Deletes multiple machine policies.
    /// </summary>
    /// <param name="requests">
    /// The requests to delete multiple machine policies.
    /// </param>
    /// <param name="retryCount">
    /// The number of retry attempts for the operation. Default is 5.
    /// </param>
    /// <returns>
    /// The results of the operation.
    /// </returns>
    public static IEnumerable<GroupPolicyDeleteResult> DeleteMultipleMachinePolicies(IEnumerable<DeleteMultipleGroupPolicyRequest> requests, int retryCount = DefaultRetryCount)
        => DeleteMultipleGroupPolicyInternal(GroupPolicyLocation.ThisComputer, string.Empty, /* isMachine */ true, requests, retryCount);
}

// User Policy (for This Computer)
partial class GroupPolicy
{
    /// <summary>
    /// Gets the user policy value from the specified subkey and value name.
    /// </summary>
    /// <param name="subKey">The subkey to retrieve the value from.</param>
    /// <param name="valueName">The name of the value to retrieve.</param>
    /// <param name="useCriticalPolicySection">Specifies whether to use the critical policy section.</param>
    /// <returns>The <see cref="GroupPolicyQueryResult"/> containing the result of the query.</returns>
    public static GroupPolicyQueryResult GetUserPolicy(string subKey, string valueName, bool useCriticalPolicySection = false)
        => GetGroupPolicyInternal(GroupPolicyLocation.ThisComputer, string.Empty, /* isMachine */ false, subKey, valueName, useCriticalPolicySection);

    /// <summary>
    /// Retrieves multiple group policy values for the current user.
    /// </summary>
    /// <param name="subKey">The subkey of the group policy.</param>
    /// <param name="valueNamePrefix">The prefix of the value names to retrieve.</param>
    /// <param name="useCriticalPolicySection">Specifies whether to use the critical policy section.</param>
    /// <returns>The result of the group policy query.</returns>
    public static MultipleGroupPolicyQueryResult GetUserPolicies(string subKey, string valueNamePrefix, bool useCriticalPolicySection = false)
        => GetGroupPoliciesInternal(GroupPolicyLocation.ThisComputer, string.Empty, /* isMachine */ false, subKey, valueNamePrefix, useCriticalPolicySection);

    /// <summary>
    /// Sets the user policy value for the specified subkey and value name.
    /// </summary>
    /// <param name="subKey">The subkey to set the value for.</param>
    /// <param name="valueName">The name of the value to set.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="requireExpandString">Specifies whether require expand string.</param>
    /// <param name="retryCount">The number of retry attempts for the operation. Default is 5.</param>
    /// <returns>The <see cref="GroupPolicyUpdateResult"/> indicating the result of the operation.</returns>
    public static GroupPolicyUpdateResult SetUserPolicy(string subKey, string valueName, object value, bool requireExpandString, int retryCount = DefaultRetryCount)
        => SetGroupPolicyInternal(GroupPolicyLocation.ThisComputer, string.Empty, /* isMachine */ false, subKey, valueName, value, requireExpandString, retryCount);

    /// <summary>
    /// Deletes the user policy value for the specified subkey and value name.
    /// </summary>
    /// <param name="subKey">The subkey to delete the value from.</param>
    /// <param name="valueName">The name of the value to delete.</param>
    /// <param name="retryCount">The number of retry attempts for the operation. Default is 5.</param>
    /// <returns>The <see cref="GroupPolicyDeleteResult"/> indicating the result of the operation.</returns>
    public static GroupPolicyDeleteResult DeleteUserPolicy(string subKey, string valueName, int retryCount = DefaultRetryCount)
        => DeleteGroupPolicyInternal(GroupPolicyLocation.ThisComputer, string.Empty, /* isMachine */ false, subKey, valueName, retryCount);

    /// <summary>
    /// Sets multiple user policies.
    /// </summary>
    /// <param name="requests">The collection of set multiple group policy requests.</param>
    /// <param name="retryCount">The number of retry attempts (default is the default retry count).</param>
    /// <returns>The collection of group policy update results.</returns>
    public static IEnumerable<GroupPolicyUpdateResult> SetMultipleUserPolicies(IEnumerable<SetMultipleGroupPolicyRequest> requests, int retryCount = DefaultRetryCount)
        => SetMultipleGroupPolicyInternal(GroupPolicyLocation.ThisComputer, string.Empty, /* isMachine */ false, requests, retryCount);

    /// <summary>
    /// Deletes multiple user policies.
    /// </summary>
    /// <param name="requests">The collection of delete multiple group policy requests.</param>
    /// <param name="retryCount">The number of retry attempts (default is the default retry count).</param>
    /// <returns>The collection of group policy delete results.</returns>
    public static IEnumerable<GroupPolicyDeleteResult> DeleteMultipleUserPolicies(IEnumerable<DeleteMultipleGroupPolicyRequest> requests, int retryCount = DefaultRetryCount)
        => DeleteMultipleGroupPolicyInternal(GroupPolicyLocation.ThisComputer, string.Empty, /* isMachine */ false, requests, retryCount);
}

// User, Group (Account) Policy (for This Computer)
partial class GroupPolicy
{
    /// <summary>
    /// Gets the account policy for the specified account.
    /// </summary>
    /// <param name="accountSid">The SID of the account.</param>
    /// <param name="subKey">The subkey of the policy.</param>
    /// <param name="valueName">The name of the value.</param>
    /// <param name="useCriticalPolicySection">Specifies whether to use the critical policy section.</param>
    /// <returns>The query result of the account policy.</returns>
    public static GroupPolicyQueryResult GetAccountPolicy(string accountSid, string subKey, string valueName, bool useCriticalPolicySection = false)
        => GetGroupPolicyInternal(GroupPolicyLocation.ThisComputersLocalUserOrGroup, accountSid, /* isMachine */ false, subKey, valueName, useCriticalPolicySection);

    /// <summary>
    /// Retrieves multiple group policy values for the specified account.
    /// </summary>
    /// <param name="accountSid">The security identifier (SID) of the account.</param>
    /// <param name="subKey">The subkey of the registry key to retrieve the values from.</param>
    /// <param name="valueNamePrefix">The prefix of the value names to retrieve.</param>
    /// <param name="useCriticalPolicySection">Specifies whether to use the critical policy section.</param>
    /// <returns>A <see cref="MultipleGroupPolicyQueryResult"/> object containing the group policy values.</returns>
    public static MultipleGroupPolicyQueryResult GetAccountPolicies(string accountSid, string subKey, string valueNamePrefix, bool useCriticalPolicySection = false)
        => GetGroupPoliciesInternal(GroupPolicyLocation.ThisComputersLocalUserOrGroup, accountSid, /* isMachine */ false, subKey, valueNamePrefix, useCriticalPolicySection);

    /// <summary>
    /// Sets the account policy for the specified account.
    /// </summary>
    /// <param name="accountSid">The SID of the account.</param>
    /// <param name="subKey">The subkey of the policy.</param>
    /// <param name="valueName">The name of the value.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="requireExpandString">Specifies whether require expand string.</param>
    /// <param name="retryCount">The number of retry attempts.</param>
    /// <returns>The operation result of setting the account policy.</returns>
    public static GroupPolicyUpdateResult SetAccountPolicy(string accountSid, string subKey, string valueName, object value, bool requireExpandString, int retryCount = DefaultRetryCount)
        => SetGroupPolicyInternal(GroupPolicyLocation.ThisComputersLocalUserOrGroup, accountSid, /* isMachine */ false, subKey, valueName, value, requireExpandString, retryCount);

    /// <summary>
    /// Deletes the account policy for the specified account.
    /// </summary>
    /// <param name="accountSid">The SID of the account.</param>
    /// <param name="subKey">The subkey of the policy.</param>
    /// <param name="valueName">The name of the value.</param>
    /// <param name="retryCount">The number of retry attempts.</param>
    /// <returns>The operation result of deleting the account policy.</returns>
    public static GroupPolicyDeleteResult DeleteAccountPolicy(string accountSid, string subKey, string valueName, int retryCount = DefaultRetryCount)
        => DeleteGroupPolicyInternal(GroupPolicyLocation.ThisComputersLocalUserOrGroup, accountSid, /* isMachine */ false, subKey, valueName, retryCount);

    /// <summary>
    /// Sets multiple account policies.
    /// </summary>
    /// <param name="accountSid">The account SID.</param>
    /// <param name="requests">The list of set multiple group policy requests.</param>
    /// <param name="retryCount">The number of retry attempts (optional).</param>
    /// <returns>The collection of group policy update results.</returns>
    public static IEnumerable<GroupPolicyUpdateResult> SetMultipleAccountPolicies(string accountSid, IEnumerable<SetMultipleGroupPolicyRequest> requests, int retryCount = DefaultRetryCount)
        => SetMultipleGroupPolicyInternal(GroupPolicyLocation.ThisComputersLocalUserOrGroup, accountSid, /* isMachine */ false, requests, retryCount);

    /// <summary>
    /// Deletes multiple account policies.
    /// </summary>
    /// <param name="accountSid">The account SID.</param>
    /// <param name="requests">The list of delete multiple group policy requests.</param>
    /// <param name="retryCount">The number of retry attempts (optional).</param>
    /// <returns>The collection of group policy delete results.</returns>
    public static IEnumerable<GroupPolicyDeleteResult> DeleteMultipleAccountPolicies(string accountSid, IEnumerable<DeleteMultipleGroupPolicyRequest> requests, int retryCount = DefaultRetryCount)
        => DeleteMultipleGroupPolicyInternal(GroupPolicyLocation.ThisComputersLocalUserOrGroup, accountSid, /* isMachine */ false, requests, retryCount);
}

// Machine Policy (for Remote Computer)
partial class GroupPolicy
{
    /// <summary>
    /// Gets the group policy for a remote machine.
    /// </summary>
    /// <param name="remoteComputerName">The name of the remote computer.</param>
    /// <param name="subKey">The subkey of the group policy.</param>
    /// <param name="valueName">The name of the value.</param>
    /// <returns>The group policy query result.</returns>
    public static GroupPolicyQueryResult GetRemoteMachinePolicy(string remoteComputerName, string subKey, string valueName)
        => GetGroupPolicyInternal(GroupPolicyLocation.RemoteComputer, remoteComputerName, /* isMachine */ true, subKey, valueName, false);

    /// <summary>
    /// Retrieves multiple group policy values from a remote machine.
    /// </summary>
    /// <param name="remoteComputerName">The name of the remote computer.</param>
    /// <param name="subKey">The subkey of the group policy.</param>
    /// <param name="valueNamePrefix">The prefix of the value names to retrieve.</param>
    /// <returns>The result of the group policy query.</returns>
    public static MultipleGroupPolicyQueryResult GetRemoteMachinePolicies(string remoteComputerName, string subKey, string valueNamePrefix)
        => GetGroupPoliciesInternal(GroupPolicyLocation.RemoteComputer, remoteComputerName, /* isMachine */ true, subKey, valueNamePrefix, false);

    /// <summary>
    /// Sets the group policy for a remote machine.
    /// </summary>
    /// <param name="remoteComputerName">The name of the remote computer.</param>
    /// <param name="subKey">The subkey of the group policy.</param>
    /// <param name="valueName">The name of the value.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="requireExpandString">Specifies whether require expand string.</param>
    /// <param name="retryCount">The number of retries.</param>
    /// <returns>The group policy operation result.</returns>
    public static GroupPolicyUpdateResult SetRemoteMachinePolicy(string remoteComputerName, string subKey, string valueName, object value, bool requireExpandString, int retryCount = DefaultRetryCount)
        => SetGroupPolicyInternal(GroupPolicyLocation.RemoteComputer, remoteComputerName, /* isMachine */ true, subKey, valueName, value, requireExpandString, retryCount);

    /// <summary>
    /// Deletes the group policy for a remote machine.
    /// </summary>
    /// <param name="remoteComputerName">The name of the remote computer.</param>
    /// <param name="subKey">The subkey of the group policy.</param>
    /// <param name="valueName">The name of the value.</param>
    /// <param name="retryCount">The number of retries.</param>
    /// <returns>The group policy operation result.</returns>
    public static GroupPolicyDeleteResult DeleteRemoteMachinePolicy(string remoteComputerName, string subKey, string valueName, int retryCount = DefaultRetryCount)
        => DeleteGroupPolicyInternal(GroupPolicyLocation.RemoteComputer, remoteComputerName, /* isMachine */ true, subKey, valueName, retryCount);

    /// <summary>
    /// Sets multiple remote machine policies.
    /// </summary>
    /// <param name="remoteComputerName">The name of the remote computer.</param>
    /// <param name="requests">The collection of set multiple group policy requests.</param>
    /// <param name="retryCount">The number of retry attempts (optional).</param>
    /// <returns>The collection of group policy update results.</returns>
    public static IEnumerable<GroupPolicyUpdateResult> SetMultipleRemoteMachinePolicies(string remoteComputerName, IEnumerable<SetMultipleGroupPolicyRequest> requests, int retryCount = DefaultRetryCount)
        => SetMultipleGroupPolicyInternal(GroupPolicyLocation.RemoteComputer, remoteComputerName, /* isMachine */ true, requests, retryCount);

    /// <summary>
    /// Deletes multiple remote machine policies.
    /// </summary>
    /// <param name="remoteComputerName">The name of the remote computer.</param>
    /// <param name="requests">The collection of delete multiple group policy requests.</param>
    /// <param name="retryCount">The number of retry attempts (optional).</param>
    /// <returns>The collection of group policy delete results.</returns>
    public static IEnumerable<GroupPolicyDeleteResult> DeleteMultipleRemoteMachinePolicies(string remoteComputerName, IEnumerable<DeleteMultipleGroupPolicyRequest> requests, int retryCount = DefaultRetryCount)
        => DeleteMultipleGroupPolicyInternal(GroupPolicyLocation.RemoteComputer, remoteComputerName, /* isMachine */ true, requests, retryCount);
}

// User Policy (for Remote Computer)
partial class GroupPolicy
{
    /// <summary>
    /// Retrieves the user policy from a remote computer.
    /// </summary>
    /// <param name="remoteComputerName">The name of the remote computer.</param>
    /// <param name="subKey">The subkey of the policy.</param>
    /// <param name="valueName">The name of the value.</param>
    /// <returns>The query result of the user policy.</returns>
    public static GroupPolicyQueryResult GetRemoteUserPolicy(string remoteComputerName, string subKey, string valueName)
        => GetGroupPolicyInternal(GroupPolicyLocation.RemoteComputer, remoteComputerName, /* isMachine */ false, subKey, valueName, false);

    /// <summary>
    /// Retrieves multiple group policy values from a remote user.
    /// </summary>
    /// <param name="remoteComputerName">The name of the remote computer.</param>
    /// <param name="subKey">The subkey of the group policy.</param>
    /// <param name="valueNamePrefix">The prefix of the value names to retrieve.</param>
    /// <returns>The result of the group policy query.</returns>
    public static MultipleGroupPolicyQueryResult GetRemoteUserPolicies(string remoteComputerName, string subKey, string valueNamePrefix)
        => GetGroupPoliciesInternal(GroupPolicyLocation.RemoteComputer, remoteComputerName, /* isMachine */ false, subKey, valueNamePrefix, false);

    /// <summary>
    /// Sets the user policy on a remote computer.
    /// </summary>
    /// <param name="remoteComputerName">The name of the remote computer.</param>
    /// <param name="subKey">The subkey of the policy.</param>
    /// <param name="valueName">The name of the value.</param>
    /// <param name="value">The value to be set.</param>
    /// <param name="requireExpandString">Specifies whether require expand string.</param>
    /// <param name="retryCount">The number of retries.</param>
    /// <returns>The operation result of setting the user policy.</returns>
    public static GroupPolicyUpdateResult SetRemoteUserPolicy(string remoteComputerName, string subKey, string valueName, object value, bool requireExpandString, int retryCount = DefaultRetryCount)
        => SetGroupPolicyInternal(GroupPolicyLocation.RemoteComputer, remoteComputerName, /* isMachine */ false, subKey, valueName, value, requireExpandString, retryCount);

    /// <summary>
    /// Deletes the user policy from a remote computer.
    /// </summary>
    /// <param name="remoteComputerName">The name of the remote computer.</param>
    /// <param name="subKey">The subkey of the policy.</param>
    /// <param name="valueName">The name of the value.</param>
    /// <param name="retryCount">The number of retries.</param>
    /// <returns>The operation result of deleting the user policy.</returns>
    public static GroupPolicyDeleteResult DeleteRemoteUserPolicy(string remoteComputerName, string subKey, string valueName, int retryCount = DefaultRetryCount)
        => DeleteGroupPolicyInternal(GroupPolicyLocation.RemoteComputer, remoteComputerName, /* isMachine */ false, subKey, valueName, retryCount);

    /// <summary>
    /// Sets multiple remote user policies.
    /// </summary>
    /// <param name="remoteComputerName">The name of the remote computer.</param>
    /// <param name="requests">The collection of set multiple group policy requests.</param>
    /// <param name="retryCount">The number of retry attempts (optional).</param>
    /// <returns>The collection of group policy update results.</returns>
    public static IEnumerable<GroupPolicyUpdateResult> SetMultipleRemoteUserPolicies(string remoteComputerName, IEnumerable<SetMultipleGroupPolicyRequest> requests, int retryCount = DefaultRetryCount)
        => SetMultipleGroupPolicyInternal(GroupPolicyLocation.RemoteComputer, remoteComputerName, /* isMachine */ false, requests, retryCount);

    /// <summary>
    /// Deletes multiple remote user policies.
    /// </summary>
    /// <param name="remoteComputerName">The name of the remote computer.</param>
    /// <param name="requests">The collection of delete multiple group policy requests.</param>
    /// <param name="retryCount">The number of retry attempts (optional).</param>
    /// <returns>The collection of group policy delete results.</returns>
    public static IEnumerable<GroupPolicyDeleteResult> DeleteMultipleRemoteUserPolicies(string remoteComputerName, IEnumerable<DeleteMultipleGroupPolicyRequest> requests, int retryCount = DefaultRetryCount)
        => DeleteMultipleGroupPolicyInternal(GroupPolicyLocation.RemoteComputer, remoteComputerName, /* isMachine */ false, requests, retryCount);
}

// Machine Policy (for Directory Service)
partial class GroupPolicy
{
    /// <summary>
    /// Gets the machine policy from the directory service.
    /// </summary>
    /// <param name="path">The path to the directory service.</param>
    /// <param name="subKey">The subkey of the policy.</param>
    /// <param name="valueName">The name of the value.</param>
    /// <returns>The query result of the machine policy.</returns>
    public static GroupPolicyQueryResult GetDirectoryMachinePolicy(string path, string subKey, string valueName)
        => GetGroupPolicyInternal(GroupPolicyLocation.DirectoryService, path, /* isMachine */ true, subKey, valueName, false);

    /// <summary>
    /// Retrieves multiple group policy values from the directory service for the specified machine.
    /// </summary>
    /// <param name="path">The path to the directory service.</param>
    /// <param name="subKey">The subkey of the group policy values.</param>
    /// <param name="valueNamePrefix">The prefix of the value names to retrieve.</param>
    /// <returns>The result of the group policy query.</returns>
    public static MultipleGroupPolicyQueryResult GetDirectoryMachinePolicies(string path, string subKey, string valueNamePrefix)
        => GetGroupPoliciesInternal(GroupPolicyLocation.DirectoryService, path, /* isMachine */ true, subKey, valueNamePrefix, false);

    /// <summary>
    /// Sets the machine policy in the directory service.
    /// </summary>
    /// <param name="path">The path to the directory service.</param>
    /// <param name="subKey">The subkey of the policy.</param>
    /// <param name="valueName">The name of the value.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="requireExpandString">Specifies whether require expand string.</param>
    /// <param name="retryCount">The number of retries.</param>
    /// <returns>The operation result of setting the machine policy.</returns>
    public static GroupPolicyUpdateResult SetDirectoryMachinePolicy(string path, string subKey, string valueName, object value, bool requireExpandString, int retryCount = DefaultRetryCount)
        => SetGroupPolicyInternal(GroupPolicyLocation.DirectoryService, path, /* isMachine */ true, subKey, valueName, value, requireExpandString, retryCount);

    /// <summary>
    /// Deletes the machine policy from the directory service.
    /// </summary>
    /// <param name="path">The path to the directory service.</param>
    /// <param name="subKey">The subkey of the policy.</param>
    /// <param name="valueName">The name of the value.</param>
    /// <param name="retryCount">The number of retries.</param>
    /// <returns>The operation result of deleting the machine policy.</returns>
    public static GroupPolicyDeleteResult DeleteDirectoryMachinePolicy(string path, string subKey, string valueName, int retryCount = DefaultRetryCount)
        => DeleteGroupPolicyInternal(GroupPolicyLocation.DirectoryService, path, /* isMachine */ true, subKey, valueName, retryCount);

    /// <summary>
    /// Sets multiple directory machine policies.
    /// </summary>
    /// <param name="path">The path of the directory.</param>
    /// <param name="requests">The collection of set multiple group policy requests.</param>
    /// <param name="retryCount">The number of retry attempts (optional, default value is DefaultRetryCount).</param>
    /// <returns>The collection of group policy update results.</returns>
    public static IEnumerable<GroupPolicyUpdateResult> SetMultipleDirectoryMachinePolicies(string path, IEnumerable<SetMultipleGroupPolicyRequest> requests, int retryCount = DefaultRetryCount)
        => SetMultipleGroupPolicyInternal(GroupPolicyLocation.DirectoryService, path, /* isMachine */ true, requests, retryCount);

    /// <summary>
    /// Deletes multiple directory machine policies.
    /// </summary>
    /// <param name="path">The path of the directory.</param>
    /// <param name="requests">The collection of delete multiple group policy requests.</param>
    /// <param name="retryCount">The number of retry attempts (optional, default value is DefaultRetryCount).</param>
    /// <returns>The collection of group policy delete results.</returns>
    public static IEnumerable<GroupPolicyDeleteResult> DeleteMultipleDirectoryMachinePolicies(string path, IEnumerable<DeleteMultipleGroupPolicyRequest> requests, int retryCount = DefaultRetryCount)
        => DeleteMultipleGroupPolicyInternal(GroupPolicyLocation.DirectoryService, path, /* isMachine */ true, requests, retryCount);
}

// User Policy (for Directory Service)
partial class GroupPolicy
{
    /// <summary>
    /// Gets the user policy from the directory service.
    /// </summary>
    /// <param name="path">The path of the directory service.</param>
    /// <param name="subKey">The subkey of the policy.</param>
    /// <param name="valueName">The name of the value.</param>
    /// <returns>The query result of the user policy.</returns>
    public static GroupPolicyQueryResult GetDirectoryUserPolicy(string path, string subKey, string valueName)
        => GetGroupPolicyInternal(GroupPolicyLocation.DirectoryService, path, /* isMachine */ false, subKey, valueName, false);

    /// <summary>
    /// Retrieves multiple group policy values for a directory user.
    /// </summary>
    /// <param name="path">The path of the directory service.</param>
    /// <param name="subKey">The subkey of the group policy.</param>
    /// <param name="valueNamePrefix">The prefix of the value names to retrieve.</param>
    /// <returns>The result of the multiple group policy query.</returns>
    public static MultipleGroupPolicyQueryResult GetDirectoryUserPolicies(string path, string subKey, string valueNamePrefix)
        => GetGroupPoliciesInternal(GroupPolicyLocation.DirectoryService, path, /* isMachine */ false, subKey, valueNamePrefix, false);

    /// <summary>
    /// Sets the user policy in the directory service.
    /// </summary>
    /// <param name="path">The path of the directory service.</param>
    /// <param name="subKey">The subkey of the policy.</param>
    /// <param name="valueName">The name of the value.</param>
    /// <param name="value">The value to be set.</param>
    /// <param name="requireExpandString">Specifies whether require expand string.</param>
    /// <param name="retryCount">The number of retry attempts.</param>
    /// <returns>The operation result of setting the user policy.</returns>
    public static GroupPolicyUpdateResult SetDirectoryUserPolicy(string path, string subKey, string valueName, object value, bool requireExpandString, int retryCount = DefaultRetryCount)
        => SetGroupPolicyInternal(GroupPolicyLocation.DirectoryService, path, /* isMachine */ false, subKey, valueName, value, requireExpandString, retryCount);

    /// <summary>
    /// Deletes the user policy from the directory service.
    /// </summary>
    /// <param name="path">The path of the directory service.</param>
    /// <param name="subKey">The subkey of the policy.</param>
    /// <param name="valueName">The name of the value.</param>
    /// <param name="retryCount">The number of retry attempts.</param>
    /// <returns>The operation result of deleting the user policy.</returns>
    public static GroupPolicyDeleteResult DeleteDirectoryUserPolicy(string path, string subKey, string valueName, int retryCount = DefaultRetryCount)
        => DeleteGroupPolicyInternal(GroupPolicyLocation.DirectoryService, path, /* isMachine */ false, subKey, valueName, retryCount);

    /// <summary>
    /// Sets multiple directory user policies.
    /// </summary>
    /// <param name="path">The path of the directory.</param>
    /// <param name="requests">The collection of set multiple group policy requests.</param>
    /// <param name="retryCount">The number of retry attempts (optional, default is DefaultRetryCount).</param>
    /// <returns>The collection of group policy update results.</returns>
    public static IEnumerable<GroupPolicyUpdateResult> SetMultipleDirectoryUserPolicies(string path, IEnumerable<SetMultipleGroupPolicyRequest> requests, int retryCount = DefaultRetryCount)
        => SetMultipleGroupPolicyInternal(GroupPolicyLocation.DirectoryService, path, /* isMachine */ false, requests, retryCount);

    /// <summary>
    /// Deletes multiple directory user policies.
    /// </summary>
    /// <param name="path">The path of the directory.</param>
    /// <param name="requests">The collection of delete multiple group policy requests.</param>
    /// <param name="retryCount">The number of retry attempts (optional, default is DefaultRetryCount).</param>
    /// <returns>The collection of group policy delete results.</returns>
    public static IEnumerable<GroupPolicyDeleteResult> DeleteMultipleDirectoryUserPolicies(string path, IEnumerable<DeleteMultipleGroupPolicyRequest> requests, int retryCount = DefaultRetryCount)
        => DeleteMultipleGroupPolicyInternal(GroupPolicyLocation.DirectoryService, path, /* isMachine */ false, requests, retryCount);
}
