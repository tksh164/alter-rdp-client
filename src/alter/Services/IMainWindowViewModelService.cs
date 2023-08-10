using System;

namespace AlterApp.Services
{
    internal interface IMainWindowViewModelService
    {
        public string BuildWindowTitle(string appName, string userNmae, string remoteComputer, string remotePort);
        public string BuildDestinationDisplayText(string userNmae, string remoteComputer, string remotePort);

        public void RdpClientHost_OnConnecting(object? sender, EventArgs e);
        public void RdpClientHost_OnConnected(object? sender, EventArgs e);
    }
}
