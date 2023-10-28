using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MsRdcAx;
using AlterApp.Services.Interfaces;
using AlterApp.ViewModels;
using AlterApp.Models;

namespace AlterApp.Services
{
    internal partial class MainWindowViewModelService : IMainWindowViewModelService
    {
        private readonly IAppSettingsService _appSettingsService;

        public MainWindowViewModelService(IAppSettingsService appSettingsService)
        {
            _appSettingsService = appSettingsService;
        }

        public string GetWindowTitle(string connectionTitle, string remoteComputer, string remotePort, string userNmae)
        {
            string trimedConnectionTitle = connectionTitle.Trim();
            string trimedRemoteComputer = remoteComputer.Trim();
            string trimedRemotePort = remotePort.Trim();
            string trimedUserNmae = userNmae.Trim();

            if (string.IsNullOrWhiteSpace(trimedConnectionTitle) && string.IsNullOrWhiteSpace(trimedRemoteComputer))
            {
                return AppConstants.AppName;
            }

            List<string> windowTitleParts = new();

            if (!string.IsNullOrWhiteSpace(trimedConnectionTitle))
            {
                windowTitleParts.Add(trimedConnectionTitle);
            }

            if (!string.IsNullOrWhiteSpace(trimedRemoteComputer) && !string.IsNullOrWhiteSpace(trimedRemotePort) && !string.IsNullOrWhiteSpace(trimedUserNmae))
            {
                windowTitleParts.Add(string.Format("{0} | {1}:{2}", trimedUserNmae, trimedRemoteComputer, trimedRemotePort));
            }
            else if (!string.IsNullOrWhiteSpace(trimedRemoteComputer) && !string.IsNullOrWhiteSpace(trimedUserNmae))
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

            windowTitleParts.Add(AppConstants.AppName);

            return string.Join(" - ", windowTitleParts);
        }

        public string GetRemoteComputerWithPort(string remoteComputer, string remotePort)
        {
            const string placeHolderText = "????";
            const string separator = ":";
            string trimedRemoteComputer = remoteComputer.Trim();
            string trimedRemotePort = remotePort.Trim();

            if (string.IsNullOrWhiteSpace(trimedRemoteComputer))
            {
                return string.Empty;
            }

            if (string.IsNullOrWhiteSpace(trimedRemotePort))
            {
                return trimedRemoteComputer + separator + placeHolderText;
            }

            return trimedRemoteComputer + separator + trimedRemotePort;
        }

        public ConnectionInfoHeaderVisibility GetConnectionHeaderVisibility(string connectionTitle, string remoteComputer, string userName)
        {
            if (!string.IsNullOrWhiteSpace(connectionTitle))
            {
                if (string.IsNullOrWhiteSpace(remoteComputer) && string.IsNullOrWhiteSpace(userName))
                {
                    return ConnectionInfoHeaderVisibility.TitleOnly;
                }

                return ConnectionInfoHeaderVisibility.TitleDestinationUserName;
            }

            if (!string.IsNullOrWhiteSpace(remoteComputer) || !string.IsNullOrWhiteSpace(userName))
            {
                return ConnectionInfoHeaderVisibility.DestinationAndUserName;
            }

            return ConnectionInfoHeaderVisibility.None;
        }

        public bool IsValidRemoteComputer(string remoteComputer)
        {
            if (string.IsNullOrWhiteSpace(remoteComputer)) return false;
            if (IPAddressRegex().Match(remoteComputer.Trim()).Success) return true;
            if (HostNameRegex().Match(remoteComputer.Trim()).Success) return true;
            return false;
        }

        [GeneratedRegex("^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
        private static partial Regex IPAddressRegex();

        [GeneratedRegex("^(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\\-]*[a-zA-Z0-9])\\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\\-]*[A-Za-z0-9])$", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
        private static partial Regex HostNameRegex();

        public bool IsValidRemotePort(string remotePort)
        {
            if (string.IsNullOrWhiteSpace(remotePort)) return false;
            if (!int.TryParse(remotePort, out int port)) return false;

            const int minPort = 1;
            const int maxPort = 65535;
            if (port < minPort || port > maxPort) return false;

            return true;
        }

        public bool IsValidUserName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) return false;
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
            string? appVersion = _appSettingsService.GetAppVersionSemanticPart();
            if (appVersion == null)
            {
                return string.Format("{0} (Could not get app version)", AppConstants.AppName);
            }
            return string.Format("{0} v{1}", AppConstants.AppName, appVersion);
        }

        public void OpenProjectWebsite()
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = AppConstants.ProjectWebsiteUri,
                UseShellExecute = true,
            });
        }

        public T GetAppSettingValue<T>(string name, T defaultValue)
        {
            return _appSettingsService.GetSettingValue(name, defaultValue);
        }

        public int SetAppSettingValue<T>(string name, T newValue)
        {
            return _appSettingsService.SetSettingValue(name, newValue);
        }
    }
}
