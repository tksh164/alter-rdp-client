using System;
using System.Windows;
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

        private RdpClientConnectionState _rdpClientConnectionState = RdpClientConnectionState.NotConnected;
        public RdpClientConnectionState RdpClientConnectionState
        {
            get => _rdpClientConnectionState;
            private set => SetProperty(ref _rdpClientConnectionState, value);
        }

        private RdpClientDisconnectReason _rdpClientLastDisconnectReason = new();
        public RdpClientDisconnectReason RdpClientLastDisconnectReason
        {
            get => _rdpClientLastDisconnectReason;
            private set
            {
                SetProperty(ref _rdpClientLastDisconnectReason, value);
                OnPropertyChanged(nameof(ShouldShowDisconnectReason));
            }
        }

        public bool ShouldShowDisconnectReason
        {
            get
            {
                return _viewModelService.ShouldShowDisconnectReason(RdpClientLastDisconnectReason);
            }
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
            RdpClientHost.RemotePort = int.Parse(RemotePort);  // TODO: Validation
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
        }

        private void SwtichToSessionSetupView()
        {
            IsElementEnabled = true;
            RdpClientHostVisibility = Visibility.Hidden;
        }

        private void RdpClientHost_OnConnecting(object? sender, EventArgs e)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            var rdpClientHost = (RdpClientHost)sender;
            RdpClientConnectionState = rdpClientHost.ConnectionState;
        }

        private void RdpClientHost_OnConnected(object? sender, EventArgs e)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            var rdpClientHost = (RdpClientHost)sender;
            RdpClientConnectionState = rdpClientHost.ConnectionState;
            RdpClientHostVisibility = Visibility.Visible;
        }

        private void RdpClientHost_OnDisconnected(object sender, IMsTscAxEvents_OnDisconnectedEvent e)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            var rdpClientHost = (RdpClientHost)sender;
            RdpClientLastDisconnectReason = rdpClientHost.LastDisconnectReason;
            RdpClientConnectionState = rdpClientHost.ConnectionState;
            SwtichToSessionSetupView();
            GC.Collect();
        }
    }
}
