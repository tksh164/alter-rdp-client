using System;
using System.Reflection;
using AlterApp.Services.Interfaces;

namespace AlterApp.Services
{
    internal class AppSettingsService : IAppSettingsService
    {
        private const string _appName = "Alter";

        public string AppName => _appName;

        public string? GetAppVersion()
        {
            return (((Assembly.GetEntryAssembly())?.GetName())?.Version)?.ToString();
        }

        public string? GetSemanticAppVersion()
        {
            string? appVersion = GetAppVersion();
            return appVersion?.Substring(0, appVersion.LastIndexOf(".", StringComparison.OrdinalIgnoreCase));
        }

        private const string _defaultRemotePort = "3389";

        public string DefaultRemotePort => _defaultRemotePort;
    }
}
