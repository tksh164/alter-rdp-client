using MsRdcAx;

namespace AlterApp.Services.Interfaces
{
    internal interface IMainWindowViewModelService
    {
        public string GetWindowTitle(string connectionNickname, string remoteComputer, string remotePort, string userNmae);

        public string GetDestinationText(string remoteComputer, string remotePort, string userNmae);

        public bool ShouldShowDestinationAndNicknameTitle(string connectionNickname);

        public bool ValidateRemoteComputer(string remoteComputer);

        public bool ValidateRemotePort(string remotePort);

        public bool ValidateUserName(string userName);

        public RdpClientHost GetRdpClientInstance();

        public bool ShouldShowDisconnectReason(RdpClientDisconnectReason lastDisconnectReason);

        public string GetVersionInfoText();

        public void OpenProjectWebsite();
    }
}
