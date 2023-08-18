using AlterApp.Services.Interfaces;

namespace AlterApp.Services
{
    internal class AppSettingsService : IAppSettingsService
    {
        private const string _appName = "Alter";

        public string AppName => _appName;

        private const string _defaultRemotePort = "3389";

        public string GetRemotePort()
        {
            return _defaultRemotePort;
        }
    }
}
