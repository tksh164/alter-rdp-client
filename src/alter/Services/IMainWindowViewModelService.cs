using MsRdcAx;

namespace AlterApp.Services
{
    internal interface IMainWindowViewModelService
    {
        public string BuildWindowTitle(string userProvidedWindowTitle, string userNmae, string remoteComputer, string remotePort, string appName);
        public string BuildDestinationDisplayText(string userNmae, string remoteComputer, string remotePort);
        public bool ShouldShowNicknameAndDestinationTitle(string userProvidedWindowTitle);
        public bool ShouldShowDisconnectReason(RdpClientDisconnectReason lastDisconnectReason);
    }
}
