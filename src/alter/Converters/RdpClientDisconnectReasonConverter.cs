using System;
using System.Globalization;
using System.Windows.Data;
using MsRdcAx;

namespace AlterApp.Converters
{
    [ValueConversion(typeof(RdpClientDisconnectReason), typeof(string))]
    internal class RdpClientDisconnectReasonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ArgumentNullException.ThrowIfNull(value, nameof(value));

            var disconnectReason = (RdpClientDisconnectReason)value;
            return disconnectReason.Reason switch
            {
                RdpClientDisconnectReasonCode.NoInfo => "No information is available.",

                RdpClientDisconnectReasonCode.LocalNotError or
                RdpClientDisconnectReasonCode.AuthenticationWarningDismissed => "Local disconnection.",

                RdpClientDisconnectReasonCode.RemoteByUser => "Remote disconnection by user.",

                RdpClientDisconnectReasonCode.ByServer => disconnectReason.ExtendedReason switch
                {
                    RdpClientExtendedDisconnectReasonCode.Shutdown => "The remote computer is shutting down.",
                    RdpClientExtendedDisconnectReasonCode.Reboot => "The remote computer is restarting.",
                    RdpClientExtendedDisconnectReasonCode.LogoffByUser => "Logoff by user.",
                    _ => "Remote disconnection by server.",
                },

                RdpClientDisconnectReasonCode.DnsLookupFailed or
                RdpClientDisconnectReasonCode.DnsLookupFailed2 => "DNS name lookup failed.",

                RdpClientDisconnectReasonCode.OutOfMemory or
                RdpClientDisconnectReasonCode.OutOfMemory2 or
                RdpClientDisconnectReasonCode.OutOfMemory3 => "Out of memory.",

                RdpClientDisconnectReasonCode.ConnectionTimedOut => "Connection timed out.",

                RdpClientDisconnectReasonCode.GetHostByNameFailed => "Windows Sockets gethostbyname call failed.",

                RdpClientDisconnectReasonCode.SocketConnectFailed => "Windows Sockets connect failed.",

                RdpClientDisconnectReasonCode.WinsockSendFailed => "Windows Sockets send call failed.",

                RdpClientDisconnectReasonCode.SocketRecvFailed => "Windows Sockets recv call failed.",

                RdpClientDisconnectReasonCode.AtClientWinsockFDCLOSE => "Socket closed.",

                RdpClientDisconnectReasonCode.HostNotFound => "Host not found error.",

                RdpClientDisconnectReasonCode.InvalidIpAddress or
                RdpClientDisconnectReasonCode.InvalidIpAddress2 => "The specified IP address is not valid.",

                RdpClientDisconnectReasonCode.InvalidSecurityData => "Security data is not valid.",

                RdpClientDisconnectReasonCode.InvalidEncryption => "The encryption method specified is not valid.",

                RdpClientDisconnectReasonCode.InvalidServerSecurityInfo => "Server security data is not valid.",

                RdpClientDisconnectReasonCode.TimeoutOccurred => "Time-out occurred.",

                RdpClientDisconnectReasonCode.ServerCertificateUnpackError => "Failed to unpack server certificate.",

                RdpClientDisconnectReasonCode.LicensingFailed => "License negotiation failed.",

                RdpClientDisconnectReasonCode.LicensingTimeout => "Licensing time-out.",

                RdpClientDisconnectReasonCode.InternalError => "Internal error.",

                RdpClientDisconnectReasonCode.TimerError => "Internal timer error.",

                RdpClientDisconnectReasonCode.InternalSecurityError or
                RdpClientDisconnectReasonCode.InternalSecurityError2 => "Internal security error.",

                RdpClientDisconnectReasonCode.EncryptionError => "Encryption error.",

                RdpClientDisconnectReasonCode.DecryptionError => "Decryption error.",

                RdpClientDisconnectReasonCode.ClientDecompressionError => "Decompression error.",

                RdpClientDisconnectReasonCode.SslErrorLogonFailure => "Login failed.",

                RdpClientDisconnectReasonCode.SslErrorNoSuchUser => "The specified user has no account.",

                RdpClientDisconnectReasonCode.SslErrorAccountDisabled => "The account is disabled.",

                RdpClientDisconnectReasonCode.SslErrorAccountRestriction => "The account is restricted.",

                RdpClientDisconnectReasonCode.SslErrorAccountLockedOut => "The account is locked out.",

                RdpClientDisconnectReasonCode.SslErrorAccountExpired => "The account is expired.",

                RdpClientDisconnectReasonCode.SslErrorPasswordExpired => "The password is expired.",

                RdpClientDisconnectReasonCode.SslErrorPasswordMustChange => "The user password must be changed before logging on for the first time.",

                RdpClientDisconnectReasonCode.SslErrorDelegationPolicy => "The policy does not support delegation of credentials to the target server.",

                RdpClientDisconnectReasonCode.SslErrorPolicyNtlmOnly => "Delegation of credentials to the target server is not allowed unless mutual authentication has been achieved.",

                RdpClientDisconnectReasonCode.SslErrorNoAuthenticatingAuthority => "No authority could be contacted for authentication.The domain name of the authenticating party could be wrong, the domain could be unreachable, or there might have been a trust relationship failure.",

                RdpClientDisconnectReasonCode.SslErrorCertExpired => "The received certificate is expired.",

                RdpClientDisconnectReasonCode.SslErrorSmartcardWrongPin => "An incorrect PIN was presented to the smart card.",

                RdpClientDisconnectReasonCode.SslErrorFreshCredRequiredByServer => "The server authentication policy does not allow connection requests using saved credentials.The user must enter new credentials.",

                RdpClientDisconnectReasonCode.SslErrorSmartcardCardBlocked => "The smart card is blocked.",

                _ => "Unknown",
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
