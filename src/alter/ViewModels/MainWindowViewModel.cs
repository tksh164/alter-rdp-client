using System;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AlterApp.Services.Interfaces;
using MsRdcAx;
using MsRdcAx.AxMsTscLib;

namespace AlterApp.ViewModels
{
    internal partial class MainWindowViewModel : ObservableObject
    {
        private readonly IMainWindowViewModelService _viewModelService;
        private readonly IAppSettingsService _appSettingsService;

        public MainWindowViewModel(IMainWindowViewModelService viewModelService, IAppSettingsService appSettingsService)
        {
            _viewModelService = viewModelService;
            _appSettingsService = appSettingsService;

            RemoteComputer = string.Empty;
            RemotePort = _appSettingsService.DefaultRemotePort;
            UserName = string.Empty;
            ConnectionNickname = string.Empty;

            RdpClientHost = _viewModelService.GetRdpClientInstance();
            RdpClientHost.OnConnecting += RdpClientHost_OnConnecting;
            RdpClientHost.OnConnected += RdpClientHost_OnConnected;
            RdpClientHost.OnDisconnected += RdpClientHost_OnDisconnected;
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(WindowTitle))]
        [NotifyPropertyChangedFor(nameof(DestinationText))]
        [NotifyCanExecuteChangedFor(nameof(ConnectToRemoteComputerCommand))]
        private string _remoteComputer;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(WindowTitle))]
        [NotifyPropertyChangedFor(nameof(DestinationText))]
        [NotifyCanExecuteChangedFor(nameof(ConnectToRemoteComputerCommand))]
        private string _remotePort;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(WindowTitle))]
        [NotifyPropertyChangedFor(nameof(DestinationText))]
        [NotifyCanExecuteChangedFor(nameof(ConnectToRemoteComputerCommand))]
        private string _userName;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(WindowTitle))]
        [NotifyPropertyChangedFor(nameof(ShouldShowDestinationAndNicknameTitle))]
        private string _connectionNickname;

        public string WindowTitle
        {
            get => _viewModelService.GetWindowTitle(ConnectionNickname, RemoteComputer, RemotePort, UserName);
        }

        public string DestinationText
        {
            get => _viewModelService.GetDestinationText(RemoteComputer, RemotePort, UserName);
        }

        public bool ShouldShowDestinationAndNicknameTitle
        {
            get => _viewModelService.ShouldShowDestinationAndNicknameTitle(ConnectionNickname);
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
            get => _viewModelService.ShouldShowDisconnectReason(RdpClientLastDisconnectReason);
        }

        private bool _shouldShowDisconnectReasonDetails = false;
        public bool ShouldShowDisconnectReasonDetails
        {
            get => _shouldShowDisconnectReasonDetails;
            private set => SetProperty(ref _shouldShowDisconnectReasonDetails, value);
        }

        [RelayCommand()]
        private void ToggleDisconnectReasonDetailsVisibility()
        {
            ShouldShowDisconnectReasonDetails = !ShouldShowDisconnectReasonDetails;
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
            SwtichToRdpClientView();
            RdpClientLastDisconnectReason = new();
            StartConnect();
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

        private void StartConnect()
        {
            if (RdpClientHost == null) throw new InvalidOperationException("The RDP client host is not instantiated.");

            RdpClientHost.RemoteComputer = RemoteComputer.Trim();
            RdpClientHost.RemotePort = int.Parse(RemotePort.Trim());
            RdpClientHost.UserName = UserName.Trim();
            RdpClientHost.DesktopWidth = (int)RdpClientHostWidth;
            RdpClientHost.DesktopHeight = (int)RdpClientHostHeight;
            RdpClientHost.Connect();
        }

        private bool CanConnectToRemoteComputer()
        {
            return _viewModelService.ValidateRemoteComputer(RemoteComputer) && _viewModelService.ValidateRemotePort(RemotePort) && _viewModelService.ValidateUserName(UserName);
        }

        public string VersionInfoText
        {
            get => _viewModelService.GetVersionInfoText();
        }

        [RelayCommand()]
        private void SetFocusToVersionInfoLink(ContentElement? element)
        {
            element?.Focus();
        }

        [RelayCommand()]
        private void OpenProjectWebsite()
        {
            _viewModelService.OpenProjectWebsite();
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
