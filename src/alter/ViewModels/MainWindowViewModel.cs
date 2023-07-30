using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AlterApp.Services;

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

        [RelayCommand(CanExecute = nameof(CanConnectToRemoteComputer))]
        private async Task ConnectToRemoteComputer()
        {
        }

        private bool CanConnectToRemoteComputer()
        {
            return !string.IsNullOrWhiteSpace(RemoteComputer) && !string.IsNullOrWhiteSpace(RemotePort) && !string.IsNullOrWhiteSpace(UserName);
        }
    }
}
