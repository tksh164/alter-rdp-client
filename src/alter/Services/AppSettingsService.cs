using System;
using System.Reflection;
using AlterApp.Services.Interfaces;

namespace AlterApp.Services
{
    internal class AppSettingsService : IAppSettingsService
    {
        private const string _appName = "Alter";

        public string AppName => _appName;

        private const string _appProjectWebsiteUri = "https://github.com/tksh164/alter-rdp-client";

        public string AppProjectWebsiteUri => _appProjectWebsiteUri;

        public string? GetAppVersion()
        {
            return (((Assembly.GetEntryAssembly())?.GetName())?.Version)?.ToString();
        }

        public string? GetAppVersionSemanticPart()
        {
            string? appVersion = GetAppVersion();
            return appVersion?.Substring(0, appVersion.LastIndexOf(".", StringComparison.OrdinalIgnoreCase));
        }

        private const string _defaultRemotePort = "3389";

        public string DefaultRemotePort => _defaultRemotePort;
    }
}
