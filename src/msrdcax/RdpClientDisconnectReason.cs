namespace MsRdcAx
{
    public class RdpClientDisconnectReason
    {
        public RdpClientDisconnectReason()
        {
            Reason = RdpClientDisconnectReasonCode.NoInfo;
            ExtendedReason = RdpClientExtendedDisconnectReasonCode.NoInfo;
            ErrorDescription = string.Empty;
        }

        public RdpClientDisconnectReason(int reason, MSTSCLib.ExtendedDisconnectReasonCode extendedReason, string errorDescription)
        {
            Reason = ConvertEnumRawValue.ToEnumMember<RdpClientDisconnectReasonCode>(reason);
            ExtendedReason = ConvertEnumRawValue.ToEnumMember<RdpClientExtendedDisconnectReasonCode>((int)extendedReason);
            ErrorDescription = errorDescription;
        }

        public RdpClientDisconnectReasonCode Reason { get; private set; } = RdpClientDisconnectReasonCode.NoInfo;

        public RdpClientExtendedDisconnectReasonCode ExtendedReason { get; private set; } = RdpClientExtendedDisconnectReasonCode.NoInfo;

        public string ErrorDescription { get; private set; }
    }

    /// <summary>
    /// The reason error codes for the disconnection.
    /// 
    /// IMsTscAxEvents::OnDisconnected method
    /// https://learn.microsoft.com/en-us/windows/win32/termserv/imstscaxevents-ondisconnected
    /// </summary>
    public enum RdpClientDisconnectReasonCode : int
    {
        NoInfo = 0x0000,                              // No information is available.
        LocalNotError = 0x0001,                       // Local disconnection. This is not an error code.
        RemoteByUser = 0x0002,                        // Remote disconnection by user. This is not an error code.
        ByServer = 0x0003,                            // Remote disconnection by server. This is not an error code.
        DnsLookupFailed = 0x0104,                     // DNS name lookup failure.
        OutOfMemory = 0x0106,                         // Out of memory.
        ConnectionTimedOut = 0x0108,                  // Connection timed out.
        SocketConnectFailed = 0x0204,                 // Windows Sockets connect failed.
        OutOfMemory2 = 0x0206,                        // Out of memory.
        HostNotFound = 0x0208,                        // Host not found error.
        WinsockSendFailed = 0x0304,                   // Windows Sockets send call failed.
        OutOfMemory3 = 0x0306,                        // Out of memory.
        InvalidIpAddress2 = 0x0308,                   // The IP address specified is not valid.
        SocketRecvFailed = 0x0404,                    // Windows Sockets recv call failed.
        InvalidSecurityData = 0x0406,                 // Security data is not valid.
        InternalError = 0x0408,                       // Internal error.
        InvalidEncryption = 0x0506,                   // The encryption method specified is not valid.
        DnsLookupFailed2 = 0x0508,                    // DNS lookup failed.
        GetHostByNameFailed = 0x0604,                 // Windows Sockets gethostbyname call failed.
        InvalidServerSecurityInfo = 0x0606,           // Server security data is not valid.
        TimerError = 0x0608,                          // Internal timer error.
        TimeoutOccurred = 0x0704,                     // Time-out occurred.
        ServerCertificateUnpackError = 0x0706,        // Failed to unpack server certificate.
        InvalidIpAddress = 0x0804,                    // Bad IP address specified.
        LicensingFailed = 0x0808,                     // License negotiation failed.
        AtClientWinsockFDCLOSE = 0x0904,              // Socket closed.
        InternalSecurityError = 0x0906,               // Internal security error.
        LicensingTimeout = 0x0908,                    // Licensing time-out.
        InternalSecurityError2 = 0x0a06,              // Internal security error.
        EncryptionError = 0x0b06,                     // Encryption error.
        DecryptionError = 0x0c06,                     // Decryption error.
        ClientDecompressionError = 0x0c08,            // Decompression error.

        SslErrorLogonFailure = 0x0807,                // Login failed.
        SslErrorNoSuchUser = 0x0a07,                  // The specified user has no account.
        SslErrorAccountDisabled = 0x0b07,             // The account is disabled.
        SslErrorAccountRestriction = 0x0c07,          // The account is restricted.
        SslErrorAccountLockedOut = 0x0d07,            // The account is locked out.
        SslErrorAccountExpired = 0x0e07,              // The account is expired.
        SslErrorPasswordExpired = 0x0f07,             // The password is expired.
        SslErrorPasswordMustChange = 0x1207,          // The user password must be changed before logging on for the first time.
        SslErrorDelegationPolicy = 0x1607,            // The policy does not support delegation of credentials to the target server.
        SslErrorPolicyNtlmOnly = 0x1707,              // Delegation of credentials to the target server is not allowed unless mutual authentication has been achieved.
        SslErrorNoAuthenticatingAuthority = 0x1807,   // No authority could be contacted for authentication.The domain name of the authenticating party could be wrong, the domain could be unreachable, or there might have been a trust relationship failure.
        SslErrorCertExpired = 0x1b07,                 // The received certificate is expired.
        SslErrorSmartcardWrongPin = 0x1c07,           // An incorrect PIN was presented to the smart card.
        SslErrorFreshCredRequiredByServer = 0x2107,   // The server authentication policy does not allow connection requests using saved credentials.The user must enter new credentials.
        SslErrorSmartcardCardBlocked = 0x2207,        // The smart card is blocked.

        AuthenticationWarningDismissed = 0x1f07,      // Authentication warning dismissed by user
    }

    /// <summary>
    /// The extended reason codes for the disconnection.
    /// 
    /// ExtendedDisconnectReasonCode enumeration
    /// https://learn.microsoft.com/en-us/windows/win32/termserv/extendeddisconnectreasoncode
    /// </summary>
    public enum RdpClientExtendedDisconnectReasonCode : int
    {
       NoInfo = 0,                                  // No additional information is available.
       APIInitiatedDisconnect = 1,                  // An application initiated the disconnection.
       APIInitiatedLogoff = 2,                      // An application logged off the client.
       ServerIdleTimeout = 3,                       // The server has disconnected the client because the client has been idle for a period of time longer than the designated time-out period.
       ServerLogonTimeout = 4,                      // The server has disconnected the client because the client has exceeded the period designated for connection.
       ReplacedByOtherConnection = 5,               // The client's connection was replaced by another connection.
       OutOfMemory = 6,                             // No memory is available.
       ServerDeniedConnection = 7,                  // The server denied the connection.
       ServerDeniedConnectionFips = 8,              // The server denied the connection for security reasons.
       ServerInsufficientPrivileges = 9,            // The server denied the connection for security reasons.
       ServerFreshCredsRequired = 10,               // Fresh credentials are required.
       RpcInitiatedDisconnectByUser = 11,           // User activity has initiated the disconnect.
       LogoffByUser = 12,                           // The user logged off, disconnecting the session.
       Shutdown = 25,                               // 
       Reboot = 26,                                 // 
       SessionLockedDueToAAD = 28,                  // 
       LicenseInternal = 256,                       // Internal licensing error.
       LicenseNoLicenseServer = 257,                // No license server was available.
       LicenseNoLicense = 258,                      // No valid software license was available.
       LicenseErrClientMsg = 259,                   // The remote computer received a licensing message that was not valid.
       LicenseHwidDoesntMatchLicense = 260,         // The hardware ID does not match the one designated on the software license.
       LicenseErrClientLicense = 261,               // Client license error.
       LicenseCantFinishProtocol = 262,             // Network problems occurred during the licensing protocol.
       LicenseClientEndedProtocol = 263,            // The client ended the licensing protocol prematurely.
       LicenseErrClientEncryption = 264,            // A licensing message was encrypted incorrectly.
       LicenseCantUpgradeLicense = 265,             // The local computer's client access license could not be upgraded or renewed.
       LicenseNoRemoteConnections = 266,            // The remote computer is not licensed to accept remote connections.
       LicenseCreatingLicStoreAccDenied = 267,      // An access denied error was received while creating a registry key for the license store.
       RdpEncInvalidCredentials = 768,              // Invalid credentials were encountered.
       ProtocolRangeStart = 4096,                   // Beginning the range of internal protocol errors. Check the server event log for additional details.
       ProtocolRangeEnd = 32767                     // Ending the range of internal protocol errors.
    }
}
