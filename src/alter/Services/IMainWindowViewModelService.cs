using MsRdcAx;

namespace AlterApp.Services
{
    internal interface IMainWindowViewModelService
    {
        public string BuildWindowTitle(string connectionNickname, string userNmae, string remoteComputer, string remotePort, string appName);
        public string BuildDestinationDisplayText(string userNmae, string remoteComputer, string remotePort);
        public bool ShouldShowConnectionNicknameAndDestinationTitle(string connectionNickname);
        public bool ShouldShowDisconnectReason(RdpClientDisconnectReason lastDisconnectReason);
    }
}
