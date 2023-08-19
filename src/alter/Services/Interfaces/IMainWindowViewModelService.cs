using MsRdcAx;

namespace AlterApp.Services.Interfaces
{
    internal interface IMainWindowViewModelService
    {
        public string GetWindowTitle(string connectionNickname, string userNmae, string remoteComputer, string remotePort);

        public string GetDestinationText(string userNmae, string remoteComputer, string remotePort);

        public bool ShouldShowDestinationAndNicknameTitle(string connectionNickname);

        public RdpClientHost GetRdpClientInstance();

        public bool ShouldShowDisconnectReason(RdpClientDisconnectReason lastDisconnectReason);

        public string GetVersionInfoText();

        public void OpenProjectWebsite();
    }
}
