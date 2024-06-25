using AdmxPolicyManager.Interop;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;

namespace AdmxPolicyManager
{
    internal static class Helpers
    {
        internal static string WellKnownSidToStringSidInternal(int sidType, SecurityIdentifier domainSid = null)
        {
            var sid = new byte[68];  // SID의 최대 크기는 68바이트로 간주
            var sidSize = sid.Length;
            var domainSidPtr = IntPtr.Zero;

            if (domainSid != null)
            {
                var domainSidBytes = new byte[domainSid.BinaryLength];
                domainSid.GetBinaryForm(domainSidBytes, 0);
                domainSidPtr = Marshal.AllocHGlobal(domainSidBytes.Length);

                Marshal.Copy(domainSidBytes, 0, domainSidPtr, domainSidBytes.Length);
            }

            var result = NativeMethods.CreateWellKnownSid(sidType, domainSidPtr, sid, ref sidSize);

            if (domainSidPtr != IntPtr.Zero)
                Marshal.FreeHGlobal(domainSidPtr);

            if (!result)
                throw new GroupPolicyManagementException($"{nameof(NativeMethods.CreateWellKnownSid)} failed.", new Win32Exception(Marshal.GetLastWin32Error()));

            if (!NativeMethods.ConvertSidToStringSidW(Marshal.UnsafeAddrOfPinnedArrayElement(sid, 0), out var stringSidPtr))
                throw new GroupPolicyManagementException($"{nameof(NativeMethods.ConvertSidToStringSidW)} failed.", new Win32Exception(Marshal.GetLastWin32Error()));

            var stringSid = Marshal.PtrToStringUni(stringSidPtr);
            NativeMethods.LocalFree(stringSidPtr);
            return stringSid;
        }

        internal static string AssertValidSid(this string sidString)
        {
            if (sidString.StartsWith("S-1-5-5-"))
                throw new GroupPolicyManagementException($"The SID '{sidString}' is the logon session ID, and this is not allowed for OpenLocalMachineGPOForPrincipal.");

            switch (sidString)
            {
                case "S-1-0-0":    // WinNullSid
                case "S-1-1-0":    // WinWorldSid
                case "S-1-2-0":    // WinLocalSid
                case "S-1-3-0":    // WinCreatorOwnerSid
                case "S-1-3-1":    // WinCreatorGroupSid
                case "S-1-3-2":    // WinCreatorOwnerServerSid
                case "S-1-3-3":    // WinCreatorGroupServerSid
                case "S-1-5":      // WinNtAuthoritySid
                case "S-1-5-1":    // WinDialupSid
                case "S-1-5-2":    // WinNetworkSid
                case "S-1-5-3":    // WinBatchSid
                case "S-1-5-4":    // WinInteractiveSid
                case "S-1-5-6":    // WinServiceSid
                case "S-1-5-7":    // WinAnonymousSid
                case "S-1-5-8":    // WinProxySid
                case "S-1-5-9":    // WinEnterpriseControllersSid
                case "S-1-5-10":   // WinSelfSid
                case "S-1-5-11":   // WinAuthenticatedUserSid
                case "S-1-5-12":   // WinRestrictedCodeSid
                case "S-1-5-13":   // WinTerminalServerSid
                case "S-1-5-14":   // WinRemoteLogonIdSid
                case "S-1-5-18":   // WinLocalSystemSid
                case "S-1-5-19":   // WinLocalServiceSid
                case "S-1-5-20":   // WinNetworkServiceSid
                case "S-1-5-32-544": // WinBuiltinAdministratorsSid
                case "S-1-5-32-545": // WinBuiltinUsersSid
                case "S-1-5-32-546": // WinBuiltinGuestsSid
                case "S-1-5-32-547": // WinBuiltinPowerUsersSid
                case "S-1-5-32-548": // WinBuiltinAccountOperatorsSid
                case "S-1-5-32-549": // WinBuiltinSystemOperatorsSid
                case "S-1-5-32-550": // WinBuiltinPrintOperatorsSid
                case "S-1-5-32-551": // WinBuiltinBackupOperatorsSid
                case "S-1-5-32-552": // WinBuiltinReplicatorsSid
                    throw new GroupPolicyManagementException($"The SID '{sidString}' is not allowed for OpenLocalMachineGPOForPrincipal.");

                default:
                    break;
            }

            return sidString;
        }

        internal static int SaveWithRetry(this IGroupPolicyObject2 gpo, bool bMachine, bool bAdd, Guid pGuidExtension, Guid pGuid, int retryCount)
        {
            var wait = 1;
            var lastException = default(Exception);
            retryCount = Math.Max(1, retryCount);

            for (var i = 0; i < retryCount; i++)
            {
                try
                {
                    return gpo.Save(bMachine, bAdd, pGuidExtension, pGuid);
                }
                catch (FileLoadException fle)
                {
                    if (unchecked((uint)fle.HResult) == 0x80070020u)
                    {
                        wait += i;
                        lastException = fle;
                        Thread.Sleep(TimeSpan.FromSeconds(wait));
                    }
                }
            }

            throw new GroupPolicyManagementException($"Cannot save the group policy object because unexpected file consumption. Retry count: {retryCount}", lastException);
        }
    }
}
