using MsRdcAx;

namespace AlterApp.Services.Interfaces
{
    internal interface IMainWindowViewModelService
    {
        public string GetWindowTitle(string connectionNickname, string userNmae, string remoteComputer, string remotePort);
        public string GetDestinationDisplayText(string userNmae, string remoteComputer, string remotePort);
        public bool ShouldShowConnectionNicknameAndDestinationTitle(string connectionNickname);
        public bool ShouldShowDisconnectReason(RdpClientDisconnectReason lastDisconnectReason);
        public string GetVersionInfoText();
    }
}
