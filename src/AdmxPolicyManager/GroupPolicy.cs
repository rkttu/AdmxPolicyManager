using AdmxParser;
using AdmxPolicyManager.Interop;
using AdmxPolicyManager.Models.Definitions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AdmxPolicyManager
{
    /// <summary>
    /// Provides methods for managing Group Policy settings.
    /// </summary>
    public static class GroupPolicy
    {
        /// <summary>
        /// The default number of times to retry saving a policy setting.
        /// </summary>
        public const int DefaultSaveRetryCount = 5;

        /// <summary>
        /// Retrieves the policy definition catalog from the system asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the policy definition catalog information.</returns>
        public static Task<PolicyDefinitionCatalogInfo> GetCatalogFromSystemAsync(CancellationToken cancellationToken = default)
            => AdmxDirectory.GetSystemPolicyDefinitions().GetCatalogAsync(cancellationToken);

        /// <summary>
        /// Retrieves the policy definition catalog from the specified ADMX directory asynchronously.
        /// </summary>
        /// <param name="admxDirectoryPath">The path to the ADMX directory.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the policy definition catalog information.</returns>
        public static Task<PolicyDefinitionCatalogInfo> GetCatalogAsync(string admxDirectoryPath, CancellationToken cancellationToken = default)
            => new AdmxDirectory(admxDirectoryPath).GetCatalogAsync(cancellationToken);

        /// <summary>
        /// Checks if the current user is an administrator.
        /// </summary>
        /// <returns><c>true</c> if the current user is an administrator; otherwise, <c>false</c>.</returns>
        public static bool IsCurrentUserAdministrator()
        {
            using (var identity = WindowsIdentity.GetCurrent())
            {
                var principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        /// <summary>
        /// Gets the security identifier (SID) for the specified user or group name.
        /// </summary>
        /// <param name="userNameOrGroupName">The name of the user or group.</param>
        /// <returns>The security identifier (SID) as a string.</returns>
        public static string GetSecurityIdentifier(string userNameOrGroupName)
            => ((SecurityIdentifier)new NTAccount(userNameOrGroupName).Translate(typeof(SecurityIdentifier))).Value;

        /// <summary>
        /// Gets the security identifier (SID) for the current user.
        /// </summary>
        /// <returns>The security identifier (SID) of the current user as a string.</returns>
        public static string GetCurrentUserSecurityIdentifier()
            => (WindowsIdentity.GetCurrent().User ??
                throw new GroupPolicyManagementException("Cannot obtain current user's security identifier.")).Value;

        /// <summary>
        /// Gets the security identifiers (SIDs) of the groups that the current user belongs to.
        /// </summary>
        /// <returns>A read-only list of key-value pairs where the key is the SID and the value is the translated name of the group.</returns>
        public static IReadOnlyList<KeyValuePair<string, string>> GetGroupSecurityIdentifiersOfCurrentUser()
        {
            return new ReadOnlyCollection<KeyValuePair<string, string>>((WindowsIdentity.GetCurrent().Groups?.Cast<SecurityIdentifier>() ??
                throw new GroupPolicyManagementException("Cannot obtain current user's security identifier."))
                .Select(x => new KeyValuePair<string, string>(x.Value, x.Translate(typeof(NTAccount)).Value))
                .ToArray());
        }

        /// <summary>
        /// Refreshes the group policy settings.
        /// </summary>
        /// <param name="force">Indicates whether to force a refresh of the policy settings.</param>
        public static void RefreshPolicy(bool force = false)
        {
            if (force)
            {
                NativeMethods.RefreshPolicyEx(true, NativeMethods.RP_FORCE);
                NativeMethods.RefreshPolicyEx(false, NativeMethods.RP_FORCE);
            }
            else
            {
                NativeMethods.RefreshPolicy(true);
                NativeMethods.RefreshPolicy(false);
            }
        }

        /// <summary>
        /// Checks if the Group Policy service is running.
        /// </summary>
        /// <returns><c>true</c> if the Group Policy service is running; otherwise, <c>false</c>.</returns>
        public static bool IsGroupPolicyServiceRunning()
        {
            using (SafeServiceHandle scManager = NativeMethods.OpenSCManager(null, null, NativeMethods.SC_MANAGER_ENUMERATE_SERVICE))
            {
                if (scManager.IsInvalid)
                    throw new GroupPolicyManagementException("Failed to open service control manager.");

                using (var service = NativeMethods.OpenServiceW(scManager, "gpsvc", NativeMethods.SERVICE_QUERY_STATUS))
                {
                    if (service.IsInvalid)
                        throw new GroupPolicyManagementException("Failed to open gpsvc service.");

                    if (!NativeMethods.QueryServiceStatus(service, out ServiceStatus status))
                        throw new GroupPolicyManagementException("Failed to query gpsvc service status.");

                    return status.dwCurrentState == NativeMethods.SERVICE_RUNNING;
                }
            }
        }

        /// <summary>
        /// Checks if the Group Policy service is configured correctly.
        /// </summary>
        /// <returns><c>true</c> if the Group Policy service is configured correctly; otherwise, <c>false</c>.</returns>
        public static bool IsGroupPolicyServiceConfiguredCorrectly()
        {
            using (var scManager = NativeMethods.OpenSCManager(null, null, NativeMethods.SC_MANAGER_ENUMERATE_SERVICE))
            {
                if (scManager.IsInvalid)
                    throw new GroupPolicyManagementException("Failed to open service control manager.");

                using (var service = NativeMethods.OpenServiceW(scManager, "gpsvc", NativeMethods.SERVICE_QUERY_CONFIG))
                {
                    if (service.IsInvalid)
                        throw new GroupPolicyManagementException("Failed to open gpsvc service.");

                    int bytesNeeded;

                    // Call QueryServiceConfig to get the size of the buffer needed
                    _ = NativeMethods.QueryServiceConfigW(service, IntPtr.Zero, 0, out bytesNeeded);

                    var configPtr = Marshal.AllocHGlobal((int)bytesNeeded);

                    if (configPtr == default)
                        throw new GroupPolicyManagementException("Cannot retrieve global heap memory.");

                    try
                    {
                        if (!NativeMethods.QueryServiceConfigW(service, configPtr, bytesNeeded, out bytesNeeded))
                            throw new GroupPolicyManagementException("Failed to query gpsvc service configuration.");

                        var config = Marshal.PtrToStructure<QueryServiceConfig>(configPtr);

                        if (config.dwStartType == NativeMethods.SERVICE_DISABLED)
                            return false;
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(configPtr);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if the specified string is a valid user or group security identifier (SID).
        /// </summary>
        /// <param name="sidString">The string to check.</param>
        /// <returns><c>true</c> if the string is a valid user or group SID; otherwise, <c>false</c>.</returns>
        public static bool IsUserOrGroupSid(string sidString)
        {
            if (!NativeMethods.ConvertStringSidToSidW(sidString, out IntPtr sidPtr))
                throw new InvalidOperationException("Invalid SID string.");

            var sidLength = NativeMethods.GetLengthSid(sidPtr);

            var sidBytes = new byte[sidLength];
            Marshal.Copy(sidPtr, sidBytes, 0, sidLength);
            Marshal.FreeHGlobal(sidPtr);

            // Lookup the account SID
            var name = new StringBuilder();
            var referencedDomainName = new StringBuilder();
            var cchName = name.Capacity;
            var cchReferencedDomainName = referencedDomainName.Capacity;

            if (!NativeMethods.LookupAccountSidW(null, sidBytes, name, ref cchName, referencedDomainName, ref cchReferencedDomainName, out SidNameUsage sidUse))
                throw new GroupPolicyManagementException("Failed to lookup SID.");

            // Check if the SID type is a user
            return (sidUse == SidNameUsage.SidTypeUser || sidUse == SidNameUsage.SidTypeGroup);
        }
    }
}
