using System;
using System.Windows;
using System.Windows.Threading;
using MsRdcAx;

namespace AlterApp.Services
{
    internal class MainWindowViewModelService : IMainWindowViewModelService
    {
        public string BuildWindowTitle(string appName, string userNmae, string remoteComputer, string remotePort)
        {
            if (!string.IsNullOrWhiteSpace(userNmae) && !string.IsNullOrWhiteSpace(remoteComputer) && !string.IsNullOrWhiteSpace(remotePort))
            {
                return string.Format("{0}@{1}:{2} - {3}", userNmae, remoteComputer, remotePort, appName);
            }
            else if (!string.IsNullOrWhiteSpace(userNmae) && !string.IsNullOrWhiteSpace(remoteComputer))
            {
                return string.Format("{0}@{1} - {2}", userNmae, remoteComputer, appName);
            }
            else if (!string.IsNullOrWhiteSpace(remoteComputer) && !string.IsNullOrWhiteSpace(remotePort))
            {
                return string.Format("{0}:{1} - {2}", remoteComputer, remotePort, appName);
            }
            else if (!string.IsNullOrWhiteSpace(remoteComputer))
            {
                return string.Format("{0} - {1}", remoteComputer, appName);
            }
            else
            {
                return appName;
            }
        }

        public string BuildDestinationDisplayText(string userNmae, string remoteComputer, string remotePort)
        {
            const string PlaceHolderText = "????";
            var userNamePart = string.IsNullOrWhiteSpace(userNmae) ? PlaceHolderText : userNmae;
            var remoteComputerPart = string.IsNullOrWhiteSpace(remoteComputer) ? PlaceHolderText : remoteComputer;
            var remotePortPart = string.IsNullOrWhiteSpace(remotePort) ? PlaceHolderText : remotePort;
            return string.Format("{0}@{1}:{2}", userNamePart, remoteComputerPart, remotePortPart);
        }

        public void RdpClientHost_OnConnecting(object? sender, EventArgs e)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            // Do hidden the WindowsFormsHost element while the RDP client establishing a connection
            // for showing the connecting status message that at under the WindowsFormsHost element.
            // If did hidden the WindowsFormsHost element at initial time, the credential prompt window does not
            // showing up the center of the main window.
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, HideRdpClientHost, sender);
        }

        private void HideRdpClientHost(RdpClientHost rdpClientHost)
        {
            rdpClientHost.Visibility = Visibility.Hidden;
        }

        public void RdpClientHost_OnConnected(object? sender, EventArgs e)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            // Do visible the WindowsFormsHost element that did hidden in the OnConnecting event handler.
            var rdpClientHost = (RdpClientHost)sender;
            rdpClientHost.Visibility = Visibility.Visible;
        }
    }
}
