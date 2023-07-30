using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AlterApp.ViewModels
{
    internal partial class MainWindowViewModel : ObservableObject
    {
        private const string _appName = "Alter";
        private const string _defaultRemotePort = "3389";

        public MainWindowViewModel()
        {
            RemoteComputer = string.Empty;
            RemotePort = _defaultRemotePort;
            UserName = string.Empty;
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(WindowTitle))]
        [NotifyPropertyChangedFor(nameof(DestinationDisplayText))]
        private string _remoteComputer;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(WindowTitle))]
        [NotifyPropertyChangedFor(nameof(DestinationDisplayText))]
        private string _remotePort;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(WindowTitle))]
        [NotifyPropertyChangedFor(nameof(DestinationDisplayText))]
        private string _userName;

        public string WindowTitle
        {
            get => _appName;
        }

        public string DestinationDisplayText
        {
            get => string.Format("{0}@{1}:{2}", UserName, RemoteComputer, RemotePort);
        }

        [RelayCommand]
        private async Task ConnectToRemoteComputer()
        { 
        }
    }
}
