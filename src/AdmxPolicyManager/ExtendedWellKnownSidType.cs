namespace AdmxPolicyManager
{
    /// <summary>
    /// Represents the extended well-known security identifier (SID) types.
    /// </summary>
    public enum ExtendedWellKnownSidType : int
    {
        /// <summary>
        /// Null SID.
        /// </summary>
        NullSid = 0,
        /// <summary>
        /// World SID.
        /// </summary>
        WorldSid = 1,
        /// <summary>
        /// Local SID.
        /// </summary>
        LocalSid = 2,
        /// <summary>
        /// Creator Owner SID.
        /// </summary>
        CreatorOwnerSid = 3,
        /// <summary>
        /// Creator Group SID.
        /// </summary>
        CreatorGroupSid = 4,
        /// <summary>
        /// Creator Owner Server SID.
        /// </summary>
        CreatorOwnerServerSid = 5,
        /// <summary>
        /// Creator Group Server SID.
        /// </summary>
        CreatorGroupServerSid = 6,
        /// <summary>
        /// NT Authority SID.
        /// </summary>
        NtAuthoritySid = 7,
        /// <summary>
        /// Dial-up SID.
        /// </summary>
        DialupSid = 8,
        /// <summary>
        /// Network SID.
        /// </summary>
        NetworkSid = 9,
        /// <summary>
        /// Batch SID.
        /// </summary>
        BatchSid = 10,
        /// <summary>
        /// Interactive SID.
        /// </summary>
        InteractiveSid = 11,
        /// <summary>
        /// Service SID.
        /// </summary>
        ServiceSid = 12,
        /// <summary>
        /// Anonymous SID.
        /// </summary>
        AnonymousSid = 13,
        /// <summary>
        /// Proxy SID.
        /// </summary>
        ProxySid = 14,
        /// <summary>
        /// Enterprise Controllers SID.
        /// </summary>
        EnterpriseControllersSid = 15,
        /// <summary>
        /// Self SID.
        /// </summary>
        SelfSid = 16,
        /// <summary>
        /// Authenticated User SID.
        /// </summary>
        AuthenticatedUserSid = 17,
        /// <summary>
        /// Restricted Code SID.
        /// </summary>
        RestrictedCodeSid = 18,
        /// <summary>
        /// Terminal Server SID.
        /// </summary>
        TerminalServerSid = 19,
        /// <summary>
        /// Remote Logon ID SID.
        /// </summary>
        RemoteLogonIdSid = 20,
        /// <summary>
        /// Logon IDs SID.
        /// </summary>
        LogonIdsSid = 21,
        /// <summary>
        /// Local System SID.
        /// </summary>
        LocalSystemSid = 22,
        /// <summary>
        /// Local Service SID.
        /// </summary>
        LocalServiceSid = 23,
        /// <summary>
        /// Network Service SID.
        /// </summary>
        NetworkServiceSid = 24,
        /// <summary>
        /// Builtin Domain SID.
        /// </summary>
        BuiltinDomainSid = 25,
        /// <summary>
        /// Builtin Administrators SID.
        /// </summary>
        BuiltinAdministratorsSid = 26,
        /// <summary>
        /// Builtin Users SID.
        /// </summary>
        BuiltinUsersSid = 27,
        /// <summary>
        /// Builtin Guests SID.
        /// </summary>
        BuiltinGuestsSid = 28,
        /// <summary>
        /// Builtin Power Users SID.
        /// </summary>
        BuiltinPowerUsersSid = 29,
        /// <summary>
        /// Builtin Account Operators SID.
        /// </summary>
        BuiltinAccountOperatorsSid = 30,
        /// <summary>
        /// Builtin System Operators SID.
        /// </summary>
        BuiltinSystemOperatorsSid = 31,
        /// <summary>
        /// Builtin Print Operators SID.
        /// </summary>
        BuiltinPrintOperatorsSid = 32,
        /// <summary>
        /// Builtin Backup Operators SID.
        /// </summary>
        BuiltinBackupOperatorsSid = 33,
        /// <summary>
        /// Builtin Replicator SID.
        /// </summary>
        BuiltinReplicatorSid = 34,
        /// <summary>
        /// Builtin Pre-Windows 2000 Compatible Access SID.
        /// </summary>
        BuiltinPreWindows2000CompatibleAccessSid = 35,
        /// <summary>
        /// Builtin Remote Desktop Users SID.
        /// </summary>
        BuiltinRemoteDesktopUsersSid = 36,
        /// <summary>
        /// Builtin Network Configuration Operators SID.
        /// </summary>
        BuiltinNetworkConfigurationOperatorsSid = 37,
        /// <summary>
        /// Account Administrator SID.
        /// </summary>
        AccountAdministratorSid = 38,
        /// <summary>
        /// Account Guest SID.
        /// </summary>
        AccountGuestSid = 39,
        /// <summary>
        /// Account Krbtgt SID.
        /// </summary>
        AccountKrbtgtSid = 40,
        /// <summary>
        /// Account Domain Admins SID.
        /// </summary>
        AccountDomainAdminsSid = 41,
        /// <summary>
        /// Account Domain Users SID.
        /// </summary>
        AccountDomainUsersSid = 42,
        /// <summary>
        /// Account Domain Guests SID.
        /// </summary>
        AccountDomainGuestsSid = 43,
        /// <summary>
        /// Account Computers SID.
        /// </summary>
        AccountComputersSid = 44,
        /// <summary>
        /// Account Controllers SID.
        /// </summary>
        AccountControllersSid = 45,
        /// <summary>
        /// Account Cert Admins SID.
        /// </summary>
        AccountCertAdminsSid = 46,
        /// <summary>
        /// Account Schema Admins SID.
        /// </summary>
        AccountSchemaAdminsSid = 47,
        /// <summary>
        /// Account Enterprise Admins SID.
        /// </summary>
        AccountEnterpriseAdminsSid = 48,
        /// <summary>
        /// Account Policy Admins SID.
        /// </summary>
        AccountPolicyAdminsSid = 49,
        /// <summary>
        /// Account RAS and IAS Servers SID.
        /// </summary>
        AccountRasAndIasServersSid = 50,
        /// <summary>
        /// NTLM Authentication SID.
        /// </summary>
        NTLMAuthenticationSid = 51,
        /// <summary>
        /// Digest Authentication SID.
        /// </summary>
        DigestAuthenticationSid = 52,
        /// <summary>
        /// SChannel Authentication SID.
        /// </summary>
        SChannelAuthenticationSid = 53,
        /// <summary>
        /// This Organization SID.
        /// </summary>
        ThisOrganizationSid = 54,
        /// <summary>
        /// Other Organization SID.
        /// </summary>
        OtherOrganizationSid = 55,
        /// <summary>
        /// Builtin Incoming Forest Trust Builders SID.
        /// </summary>
        BuiltinIncomingForestTrustBuildersSid = 56,
        /// <summary>
        /// Builtin Perf Monitoring Users SID.
        /// </summary>
        BuiltinPerfMonitoringUsersSid = 57,
        /// <summary>
        /// Builtin Perf Logging Users SID.
        /// </summary>
        BuiltinPerfLoggingUsersSid = 58,
        /// <summary>
        /// Builtin Authorization Access SID.
        /// </summary>
        BuiltinAuthorizationAccessSid = 59,
        /// <summary>
        /// Builtin Terminal Server License Servers SID.
        /// </summary>
        BuiltinTerminalServerLicenseServersSid = 60,
        /// <summary>
        /// Builtin DCOM Users SID.
        /// </summary>
        BuiltinDCOMUsersSid = 61,
        /// <summary>
        /// Builtin IUsers SID.
        /// </summary>
        BuiltinIUsersSid = 62,
        /// <summary>
        /// IUser SID.
        /// </summary>
        IUserSid = 63,
        /// <summary>
        /// Builtin Crypto Operators SID.
        /// </summary>
        BuiltinCryptoOperatorsSid = 64,
        /// <summary>
        /// Untrusted Label SID.
        /// </summary>
        UntrustedLabelSid = 65,
        /// <summary>
        /// Low Label SID.
        /// </summary>
        LowLabelSid = 66,
        /// <summary>
        /// Medium Label SID.
        /// </summary>
        MediumLabelSid = 67,
        /// <summary>
        /// High Label SID.
        /// </summary>
        HighLabelSid = 68,
        /// <summary>
        /// System Label SID.
        /// </summary>
        SystemLabelSid = 69,
        /// <summary>
        /// Write Restricted Code SID.
        /// </summary>
        WriteRestrictedCodeSid = 70,
        /// <summary>
        /// Creator Owner Rights SID.
        /// </summary>
        CreatorOwnerRightsSid = 71,
        /// <summary>
        /// Cacheable Principals Group SID.
        /// </summary>
        CacheablePrincipalsGroupSid = 72,
        /// <summary>
        /// Non-Cacheable Principals Group SID.
        /// </summary>
        NonCacheablePrincipalsGroupSid = 73,
        /// <summary>
        /// Enterprise Readonly Controllers SID.
        /// </summary>
        EnterpriseReadonlyControllersSid = 74,
        /// <summary>
        /// Account Readonly Controllers SID.
        /// </summary>
        AccountReadonlyControllersSid = 75,
        /// <summary>
        /// Builtin Event Log Readers Group.
        /// </summary>
        BuiltinEventLogReadersGroup = 76,
        /// <summary>
        /// New Enterprise Readonly Controllers SID.
        /// </summary>
        NewEnterpriseReadonlyControllersSid = 77,
        /// <summary>
        /// Builtin CertSvc DCom Access Group.
        /// </summary>
        BuiltinCertSvcDComAccessGroup = 78,
        /// <summary>
        /// Medium Plus Label SID.
        /// </summary>
        MediumPlusLabelSid = 79,
        /// <summary>
        /// Local Logon SID.
        /// </summary>
        LocalLogonSid = 80,
        /// <summary>
        /// Console Logon SID.
        /// </summary>
        ConsoleLogonSid = 81,
        /// <summary>
        /// This Organization Certificate SID.
        /// </summary>
        ThisOrganizationCertificateSid = 82,
        /// <summary>
        /// Application Package Authority SID.
        /// </summary>
        ApplicationPackageAuthoritySid = 83,
        /// <summary>
        /// Builtin Any Package SID.
        /// </summary>
        BuiltinAnyPackageSid = 84,
        /// <summary>
        /// Capability Internet Client SID.
        /// </summary>
        CapabilityInternetClientSid = 85,
        /// <summary>
        /// Capability Internet Client Server SID.
        /// </summary>
        CapabilityInternetClientServerSid = 86,
        /// <summary>
        /// Capability Private Network Client Server SID.
        /// </summary>
        CapabilityPrivateNetworkClientServerSid = 87,
        /// <summary>
        /// Capability Pictures Library SID.
        /// </summary>
        CapabilityPicturesLibrarySid = 88,
        /// <summary>
        /// Capability Videos Library SID.
        /// </summary>
        CapabilityVideosLibrarySid = 89,
        /// <summary>
        /// Capability Music Library SID.
        /// </summary>
        CapabilityMusicLibrarySid = 90,
        /// <summary>
        /// Capability Documents Library SID.
        /// </summary>
        CapabilityDocumentsLibrarySid = 91,
        /// <summary>
        /// Capability Shared User Certificates SID.
        /// </summary>
        CapabilitySharedUserCertificatesSid = 92,
        /// <summary>
        /// Capability Enterprise Authentication SID.
        /// </summary>
        CapabilityEnterpriseAuthenticationSid = 93,
        /// <summary>
        /// Capability Removable Storage SID.
        /// </summary>
        CapabilityRemovableStorageSid = 94,
        /// <summary>
        /// Builtin RDS Remote Access Servers SID.
        /// </summary>
        BuiltinRDSRemoteAccessServersSid = 95,
        /// <summary>
        /// Builtin RDS Endpoint Servers SID.
        /// </summary>
        BuiltinRDSEndpointServersSid = 96,
        /// <summary>
        /// Builtin RDS Management Servers SID.
        /// </summary>
        BuiltinRDSManagementServersSid = 97,
        /// <summary>
        /// User Mode Drivers SID.
        /// </summary>
        UserModeDriversSid = 98,
        /// <summary>
        /// Builtin Hyper-V Admins SID.
        /// </summary>
        BuiltinHyperVAdminsSid = 99,
        /// <summary>
        /// Account Cloneable Controllers SID.
        /// </summary>
        AccountCloneableControllersSid = 100,
        /// <summary>
        /// Builtin Access Control Assistance Operators SID.
        /// </summary>
        BuiltinAccessControlAssistanceOperatorsSid = 101,
        /// <summary>
        /// Builtin Remote Management Users SID.
        /// </summary>
        BuiltinRemoteManagementUsersSid = 102,
        /// <summary>
        /// Authentication Authority Asserted SID.
        /// </summary>
        AuthenticationAuthorityAssertedSid = 103,
        /// <summary>
        /// Authentication Service Asserted SID.
        /// </summary>
        AuthenticationServiceAssertedSid = 104,
        /// <summary>
        /// Local Account SID.
        /// </summary>
        LocalAccountSid = 105,
        /// <summary>
        /// Local Account and Administrator SID.
        /// </summary>
        LocalAccountAndAdministratorSid = 106,
        /// <summary>
        /// Account Protected Users SID.
        /// </summary>
        AccountProtectedUsersSid = 107,
        /// <summary>
        /// Capability Appointments SID.
        /// </summary>
        CapabilityAppointmentsSid = 108,
        /// <summary>
        /// Capability Contacts SID.
        /// </summary>
        CapabilityContactsSid = 109,
        /// <summary>
        /// Account Default System Managed SID.
        /// </summary>
        AccountDefaultSystemManagedSid = 110,
        /// <summary>
        /// Builtin Default System Managed Group SID.
        /// </summary>
        BuiltinDefaultSystemManagedGroupSid = 111,
        /// <summary>
        /// Builtin Storage Replica Admins SID.
        /// </summary>
        BuiltinStorageReplicaAdminsSid = 112,
        /// <summary>
        /// Account Key Admins SID.
        /// </summary>
        AccountKeyAdminsSid = 113,
        /// <summary>
        /// Account Enterprise Key Admins SID.
        /// </summary>
        AccountEnterpriseKeyAdminsSid = 114,
        /// <summary>
        /// Authentication Key Trust SID.
        /// </summary>
        AuthenticationKeyTrustSid = 115,
        /// <summary>
        /// Authentication Key Property MFA SID.
        /// </summary>
        AuthenticationKeyPropertyMFASid = 116,
        /// <summary>
        /// Authentication Key Property Attestation SID.
        /// </summary>
        AuthenticationKeyPropertyAttestationSid = 117,
        /// <summary>
        /// Authentication Fresh Key Auth SID.
        /// </summary>
        AuthenticationFreshKeyAuthSid = 118,
        /// <summary>
        /// Builtin Device Owners SID.
        /// </summary>
        BuiltinDeviceOwnersSid = 119,
        /// <summary>
        /// Builtin User Mode Hardware Operators SID.
        /// </summary>
        BuiltinUserModeHardwareOperatorsSid,
        /// <summary>
        /// Builtin OpenSSH Users SID.
        /// </summary>
        BuiltinOpenSSHUsersSid
    }
}
