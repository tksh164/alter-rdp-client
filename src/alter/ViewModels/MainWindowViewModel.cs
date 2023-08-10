using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AlterApp.Services;
using MsRdcAx;
using System.Windows.Threading;
using System;

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
        private async Task ConnectToRemoteComputer()
        {
            // TODO: Release RDP client host.
            if (RdpClientHost == null)
            {
                RdpClientHostVisibility = Visibility.Visible;

                RdpClientHost = new RdpClientHost
                {
                    RemoteComputer = RemoteComputer,
                    RemotePort = int.Parse(RemotePort),  // TODO
                    UserName = UserName,
                    DesktopWidth = (int)RdpClientHostWidth,
                    DesktopHeight = (int)RdpClientHostHeight,
                };

                RdpClientHost.OnConnecting += _viewModelService.RdpClientHost_OnConnecting;
                RdpClientHost.OnConnected += _viewModelService.RdpClientHost_OnConnected;

                RdpClientHost.Connect();
            }
        }

        private bool CanConnectToRemoteComputer()
        {
            return !string.IsNullOrWhiteSpace(RemoteComputer) && !string.IsNullOrWhiteSpace(RemotePort) && !string.IsNullOrWhiteSpace(UserName);
        }
    }
}
