using System.Collections.Generic;
using AlterApp.Services.Interfaces;
using MsRdcAx;

namespace AlterApp.Services
{
    internal class MainWindowViewModelService : IMainWindowViewModelService
    {
        private readonly IAppSettingsService _appSettingsService;

        public MainWindowViewModelService(IAppSettingsService appSettingsService)
        {
            _appSettingsService = appSettingsService;
        }

        public string GetWindowTitle(string connectionNickname, string userNmae, string remoteComputer, string remotePort)
        {
            if (string.IsNullOrWhiteSpace(connectionNickname) && string.IsNullOrWhiteSpace(remoteComputer))
            {
                return _appSettingsService.AppName;
            }

            List<string> windowTitleParts = new();

            if (!string.IsNullOrWhiteSpace(connectionNickname))
            {
                windowTitleParts.Add(connectionNickname);
            }

            if (!string.IsNullOrWhiteSpace(userNmae) && !string.IsNullOrWhiteSpace(remoteComputer) && !string.IsNullOrWhiteSpace(remotePort))
            {
                windowTitleParts.Add(string.Format("{0} | {1}:{2}", userNmae, remoteComputer, remotePort));
            }
            else if (!string.IsNullOrWhiteSpace(userNmae) && !string.IsNullOrWhiteSpace(remoteComputer))
            {
                windowTitleParts.Add(string.Format("{0} | {1}", userNmae, remoteComputer));
            }
            else if (!string.IsNullOrWhiteSpace(remoteComputer) && !string.IsNullOrWhiteSpace(remotePort))
            {
                windowTitleParts.Add(string.Format("{0}:{1}", remoteComputer, remotePort));
            }
            else if (!string.IsNullOrWhiteSpace(remoteComputer))
            {
                windowTitleParts.Add(remoteComputer);
            }

            windowTitleParts.Add(_appSettingsService.AppName);

            return string.Join(" - ", windowTitleParts);
        }

        public string GetDestinationDisplayText(string userNmae, string remoteComputer, string remotePort)
        {
            if (string.IsNullOrWhiteSpace(userNmae) && string.IsNullOrWhiteSpace(remoteComputer))
            {
                return string.Empty;
            }

            const string placeHolderText = "????";
            var userNamePart = string.IsNullOrWhiteSpace(userNmae) ? placeHolderText : userNmae;
            var remoteComputerPart = string.IsNullOrWhiteSpace(remoteComputer) ? placeHolderText : remoteComputer;
            var remotePortPart = string.IsNullOrWhiteSpace(remotePort) ? placeHolderText : remotePort;
            return string.Format("{0} | {1}:{2}", userNamePart, remoteComputerPart, remotePortPart);
        }

        public bool ShouldShowConnectionNicknameAndDestinationTitle(string connectionNickname)
        {
            return !string.IsNullOrWhiteSpace(connectionNickname);
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
    }
}
