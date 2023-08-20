using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MsRdcAx;
using AlterApp.Services.Interfaces;

namespace AlterApp.Services
{
    internal partial class MainWindowViewModelService : IMainWindowViewModelService
    {
        private readonly IAppSettingsService _appSettingsService;

        public MainWindowViewModelService(IAppSettingsService appSettingsService)
        {
            _appSettingsService = appSettingsService;
        }

        public string GetWindowTitle(string connectionNickname, string remoteComputer, string remotePort, string userNmae)
        {
            string trimedConnectionNickname = connectionNickname.Trim();
            string trimedRemoteComputer = remoteComputer.Trim();
            string trimedRemotePort = remotePort.Trim();
            string trimedUserNmae = userNmae.Trim();

            if (string.IsNullOrWhiteSpace(trimedConnectionNickname) && string.IsNullOrWhiteSpace(trimedRemoteComputer))
            {
                return _appSettingsService.AppName;
            }

            List<string> windowTitleParts = new();

            if (!string.IsNullOrWhiteSpace(trimedConnectionNickname))
            {
                windowTitleParts.Add(trimedConnectionNickname);
            }

            if (!string.IsNullOrWhiteSpace(trimedUserNmae) && !string.IsNullOrWhiteSpace(trimedRemoteComputer) && !string.IsNullOrWhiteSpace(trimedRemotePort))
            {
                windowTitleParts.Add(string.Format("{0} | {1}:{2}", trimedUserNmae, trimedRemoteComputer, trimedRemotePort));
            }
            else if (!string.IsNullOrWhiteSpace(trimedUserNmae) && !string.IsNullOrWhiteSpace(trimedRemoteComputer))
            {
                windowTitleParts.Add(string.Format("{0} | {1}", trimedUserNmae, trimedRemoteComputer));
            }
            else if (!string.IsNullOrWhiteSpace(trimedRemoteComputer) && !string.IsNullOrWhiteSpace(trimedRemotePort))
            {
                windowTitleParts.Add(string.Format("{0}:{1}", trimedRemoteComputer, trimedRemotePort));
            }
            else if (!string.IsNullOrWhiteSpace(trimedRemoteComputer))
            {
                windowTitleParts.Add(trimedRemoteComputer);
            }

            windowTitleParts.Add(_appSettingsService.AppName);

            return string.Join(" - ", windowTitleParts);
        }

        public string GetDestinationText(string userNmae, string remoteComputer, string remotePort)
        {
            string trimedRemoteComputer = remoteComputer.Trim();
            string trimedRemotePort = remotePort.Trim();
            string trimedUserNmae = userNmae.Trim();

            if (string.IsNullOrWhiteSpace(trimedUserNmae) && string.IsNullOrWhiteSpace(trimedRemoteComputer))
            {
                return string.Empty;
            }

            const string placeHolderText = "????";
            var userNamePart = string.IsNullOrWhiteSpace(trimedUserNmae) ? placeHolderText : trimedUserNmae;
            var remoteComputerPart = string.IsNullOrWhiteSpace(trimedRemoteComputer) ? placeHolderText : trimedRemoteComputer;
            var remotePortPart = string.IsNullOrWhiteSpace(trimedRemotePort) ? placeHolderText : trimedRemotePort;
            return string.Format("{0} | {1}:{2}", userNamePart, remoteComputerPart, remotePortPart);
        }

        public bool ShouldShowDestinationAndNicknameTitle(string connectionNickname)
        {
            return !string.IsNullOrWhiteSpace(connectionNickname);
        }

        public bool ValidateRemoteComputer(string remoteComputer)
        {
            if (string.IsNullOrWhiteSpace(remoteComputer)) return false;
            if (IPAddressRegex().Match(remoteComputer.Trim()).Success) return true;
            if (HostNameRegex().Match(remoteComputer.Trim()).Success) return true;
            return false;
        }

        [GeneratedRegex("^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
        private static partial Regex IPAddressRegex();

        [GeneratedRegex("^(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\\-]*[a-zA-Z0-9])\\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\\-]*[A-Za-z0-9])$", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
        private static partial Regex HostNameRegex();

        public bool ValidateRemotePort(string remotePort)
        {
            if (string.IsNullOrWhiteSpace(remotePort)) return false;
            if (!int.TryParse(remotePort, out int port)) return false;

            const int minPort = 1;
            const int maxPort = 65535;
            if (port < minPort || port > maxPort) return false;

            return true;
        }

        public bool ValidateUserName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) return false;
            if (0 <= userName.Trim().IndexOf(' ', StringComparison.OrdinalIgnoreCase)) return false;
            return true;
        }

        public RdpClientHost GetRdpClientInstance()
        {
            return new RdpClientHost();
        }

        public bool ShouldShowDisconnectReason(RdpClientDisconnectReason lastDisconnectReason)
        {
            switch (lastDisconnectReason.Reason)
            {
                case RdpClientDisconnectReasonCode.NoInfo:
                    return lastDisconnectReason.ExtendedReason switch
                    {
                        RdpClientExtendedDisconnectReasonCode.NoInfo => false,
                        _ => true,
                    };
                case RdpClientDisconnectReasonCode.LocalNotError:
                    return lastDisconnectReason.ExtendedReason switch
                    {
                        RdpClientExtendedDisconnectReasonCode.NoInfo => false,
                        _ => true,
                    };
                //case RdpClientDisconnectReasonCode.RemoteByUser:
                //    return lastDisconnectReason.ExtendedReason switch
                //    {
                //        RdpClientExtendedDisconnectReasonCode.RpcInitiatedDisconnectByUser => false,
                //        RdpClientExtendedDisconnectReasonCode.LogoffByUser => false,
                //        _ => true,
                //    };
                case RdpClientDisconnectReasonCode.AuthenticationWarningDismissed:
                    return false;
                default:
                    return true;
            }
        }

        public string GetVersionInfoText()
        {
            string? appVersion = _appSettingsService.GetSemanticAppVersion();
            if (appVersion == null)
            {
                return string.Format("{0} (Could not get app version)", _appSettingsService.AppName);
            }
            return string.Format("{0} v{1}", _appSettingsService.AppName, appVersion);
        }

        public void OpenProjectWebsite()
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = _appSettingsService.AppProjectWebsiteUri,
                UseShellExecute = true,
            });
        }
    }
}
