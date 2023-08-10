using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AlterApp.Services;
using MsRdcAx;

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

        private RdpClientHost? _rdpClientHost = null;
        public RdpClientHost? RdpClientHost
        {
            get => _rdpClientHost;
            private set => SetProperty(ref _rdpClientHost, value);
        }

        public string WindowTitle
        {
            get => _viewModelService.BuildWindowTitle(_appSettingsService.GetAppName(), UserName, RemoteComputer, RemotePort);
        }

        public string DestinationDisplayText
        {
            get => _viewModelService.BuildDestinationDisplayText(UserName, RemoteComputer, RemotePort);
        }

        private Visibility _rdpClientHostVisibility = Visibility.Hidden;
        public Visibility RdpClientHostVisibility
        {
            get => _rdpClientHostVisibility;
            private set => SetProperty(ref _rdpClientHostVisibility, value);
        }

        [ObservableProperty]
        private double _rdpClientHostWidth;

        [ObservableProperty]
        private double _rdpClientHostHeight;

        // TODO: Enable/Disable flag for elements

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
                RdpClientHost.Connect();
            }
        }

        private bool CanConnectToRemoteComputer()
        {
            return !string.IsNullOrWhiteSpace(RemoteComputer) && !string.IsNullOrWhiteSpace(RemotePort) && !string.IsNullOrWhiteSpace(UserName);
        }
    }
}
