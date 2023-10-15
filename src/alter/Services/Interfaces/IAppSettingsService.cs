namespace AlterApp.Services.Interfaces
{
    internal interface IAppSettingsService
    {
        public string AppName { get; }

        public string AppProjectWebsiteUri { get; }

        public string? GetAppVersion();

        public string? GetAppVersionSemanticPart();

        public string DefaultRemotePort { get; }
    }
}
