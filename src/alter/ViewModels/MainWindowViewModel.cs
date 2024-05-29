using System;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MsRdcAx;
using MsRdcAx.AxMsTscLib;
using AlterApp.Services.Interfaces;
using AlterApp.ViewModels.Interfaces;
using AlterApp.Models;

namespace AlterApp.ViewModels
{
    internal partial class MainWindowViewModel : ObservableObject, IWindowClosing, IWindowContentRendered
    {
        private readonly IMainWindowViewModelService _viewModelService;

        public MainWindowViewModel(IMainWindowViewModelService viewModelService, ICommandLineArgsService commandLineArgsService, IUsageNoticeService usageNoticeService)
        {
            _viewModelService = viewModelService;

            RdpClientHost = _viewModelService.GetRdpClientInstance();
            RdpClientHost.OnConnecting += RdpClientHost_OnConnecting;
            RdpClientHost.OnConnected += RdpClientHost_OnConnected;
            RdpClientHost.OnDisconnected += RdpClientHost_OnDisconnected;

            WindowWidth = _viewModelService.GetAppSettingValue("mainWindow.width", AppConstants.DefaultMainWindowWidth);
            WindowHeight = _viewModelService.GetAppSettingValue("mainWindow.height", AppConstants.DefaultMainWindowHeight);

            RemoteComputer = commandLineArgsService.RemoteComputer ?? string.Empty;
            RemotePort = commandLineArgsService.RemotePort ?? _viewModelService.GetAppSettingValue("defaultRdpPort", AppConstants.DefaultRdpPort).ToString();
            UserName = commandLineArgsService.UserName ?? string.Empty;
            ConnectionTitle = commandLineArgsService.ConnectionTitle ?? string.Empty;

            // NOTE: We cannot start connect at this time. Must start connect after the window content is rendered because the RDP client's
            // DesktopWidth and DesktopHeight comes from the ContentControl element size via the RdpClientHostWidth and RdpClientHostHeight properties.
            // The ContentControl element size does not get until the window content is rendered (We cannot get it at this time).
            _shouldStartConnect = !commandLineArgsService.ShouldShowUsage && commandLineArgsService.ShouldStartConnect;

            if (commandLineArgsService.ShouldShowUsage)
            {
                usageNoticeService.ShowUsage();
            }
        }

        public bool OnClosing()
        {
            _viewModelService.SetAppSettingValue("mainWindow.width", WindowWidth);
            _viewModelService.SetAppSettingValue("mainWindow.height", WindowHeight);
            return false;
        }

        private readonly bool _shouldStartConnect;

        public void OnContentRendered()
        {
            // Start connect if the command line arguments indicate to do so.
            if (_shouldStartConnect && ConnectToRemoteComputerCommand.CanExecute(null))
            {
                ConnectToRemoteComputerCommand.Execute(null);
            }
        }

        [ObservableProperty]
        private double _windowWidth;

        [ObservableProperty]
        private double _windowHeight;

        public static double WindowMinWidth => AppConstants.MainWindowMinWidth;

        public static double WindowMinHeight => AppConstants.MainWindowMinHeight;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(WindowTitle))]
        [NotifyCanExecuteChangedFor(nameof(ConnectToRemoteComputerCommand))]
        private string _remoteComputer = string.Empty;

        private string _remotePort = string.Empty;
        public string RemotePort
        {
            get => _remotePort;
            set
            {
                if (value.Length == 0 || _viewModelService.IsValidRemotePort(value))
                {
                    SetProperty(ref _remotePort, value);
                    OnPropertyChanged(nameof(WindowTitle));
                    ConnectToRemoteComputerCommand.NotifyCanExecuteChanged();
                }
            }
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(WindowTitle))]
        [NotifyCanExecuteChangedFor(nameof(ConnectToRemoteComputerCommand))]
        private string _userName = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(WindowTitle))]
        private string _connectionTitle = string.Empty;

        public string WindowTitle => _viewModelService.GetWindowTitle(ConnectionTitle, RemoteComputer, RemotePort, UserName);

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

        public bool ShouldShowDisconnectReason => _viewModelService.ShouldShowDisconnectReason(RdpClientLastDisconnectReason);

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
            return _viewModelService.IsValidRemoteComputer(RemoteComputer) &&
                   _viewModelService.IsValidRemotePort(RemotePort) &&
                   _viewModelService.IsValidUserName(UserName);
        }

        public string VersionInfoText => _viewModelService.GetVersionInfoText();

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
