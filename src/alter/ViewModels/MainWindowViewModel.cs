using System;
using System.Windows;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AlterApp.Services;
using MsRdcAx;
using MsRdcAx.AxMsTscLib;

namespace AlterApp.ViewModels
{
    internal partial class MainWindowViewModel : ObservableObject
    {
        private readonly IAppSettingsService _appSettingsService;
        private readonly IMainWindowViewModelService _viewModelService;

        public MainWindowViewModel(IAppSettingsService appSettingsService, IMainWindowViewModelService viewModelService)
        {
            _appSettingsService = appSettingsService;
            _viewModelService = viewModelService;

            RemoteComputer = string.Empty;
            RemotePort = _appSettingsService.GetRemotePort();
            UserName = string.Empty;

            RdpClientHost = new RdpClientHost();
            RdpClientHost.OnConnecting += RdpClientHost_OnConnecting;
            RdpClientHost.OnConnected += RdpClientHost_OnConnected;
            RdpClientHost.OnDisconnected += RdpClientHost_OnDisconnected;
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(WindowTitle))]
        [NotifyPropertyChangedFor(nameof(DestinationDisplayText))]
        [NotifyCanExecuteChangedFor(nameof(ConnectToRemoteComputerCommand))]
        private string _remoteComputer;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(WindowTitle))]
        [NotifyPropertyChangedFor(nameof(DestinationDisplayText))]
        [NotifyCanExecuteChangedFor(nameof(ConnectToRemoteComputerCommand))]
        private string _remotePort;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(WindowTitle))]
        [NotifyPropertyChangedFor(nameof(DestinationDisplayText))]
        [NotifyCanExecuteChangedFor(nameof(ConnectToRemoteComputerCommand))]
        private string _userName;

        public string WindowTitle
        {
            get => _viewModelService.BuildWindowTitle(_appSettingsService.GetAppName(), UserName, RemoteComputer, RemotePort);
        }

        public string DestinationDisplayText
        {
            get => _viewModelService.BuildDestinationDisplayText(UserName, RemoteComputer, RemotePort);
        }

        private RdpClientHost? _rdpClientHost = null;
        public RdpClientHost? RdpClientHost
        {
            get => _rdpClientHost;
            private set => SetProperty(ref _rdpClientHost, value);
        }

        [ObservableProperty]
        private double _rdpClientHostWidth;

        [ObservableProperty]
        private double _rdpClientHostHeight;

        private RdpClientDisconnectReason _rdpClientLastDisconnectReason = new();
        public RdpClientDisconnectReason RdpClientLastDisconnectReason
        {
            get => _rdpClientLastDisconnectReason;
            private set => SetProperty(ref _rdpClientLastDisconnectReason, value);
        }

        private bool _isElementEnabled = true;
        public bool IsElementEnabled
        {
            get => _isElementEnabled;
            private set => SetProperty(ref _isElementEnabled, value);
        }

        private Visibility _rdpClientHostVisibility = Visibility.Hidden;
        public Visibility RdpClientHostVisibility
        {
            get => _rdpClientHostVisibility;
            private set => SetProperty(ref _rdpClientHostVisibility, value);
        }

        [RelayCommand(CanExecute = nameof(CanConnectToRemoteComputer))]
        private void ConnectToRemoteComputer()
        {
            if (RdpClientHost == null) throw new InvalidOperationException("The RDP client host is not instantiated.");

            SwtichToRdpClientView();
            RdpClientHost.RemoteComputer = RemoteComputer;
            RdpClientHost.RemotePort = int.Parse(RemotePort);  // TODO: Valication
            RdpClientHost.UserName = UserName;
            RdpClientHost.DesktopWidth = (int)RdpClientHostWidth;
            RdpClientHost.DesktopHeight = (int)RdpClientHostHeight;
            RdpClientHost.Connect();
        }

        private bool CanConnectToRemoteComputer()
        {
            return !string.IsNullOrWhiteSpace(RemoteComputer) && !string.IsNullOrWhiteSpace(RemotePort) && !string.IsNullOrWhiteSpace(UserName);
        }

        private void SwtichToRdpClientView()
        {
            IsElementEnabled = false;
            RdpClientHostVisibility = Visibility.Visible;
        }

        private void SwtichToSessionSetupView()
        {
            IsElementEnabled = true;
            RdpClientHostVisibility = Visibility.Hidden;
        }

        private void RdpClientHost_OnConnecting(object? sender, EventArgs e)
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

        private void RdpClientHost_OnConnected(object? sender, EventArgs e)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            // Do visible the WindowsFormsHost element that did hidden in the OnConnecting event handler.
            var rdpClientHost = (RdpClientHost)sender;
            rdpClientHost.Visibility = Visibility.Visible;
        }

        private void RdpClientHost_OnDisconnected(object sender, IMsTscAxEvents_OnDisconnectedEvent e)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            var rdpClientHost = (RdpClientHost)sender;
            RdpClientLastDisconnectReason = rdpClientHost.LastDisconnectReason;
            SwtichToSessionSetupView();
            GC.Collect();

            //ConnectionState = RdpClientHost.ConnectionState;
            //ConnectCommand.NotifyCanExecuteChanged();
            //DisconnectCommand.NotifyCanExecuteChanged();
        }
    }
}
