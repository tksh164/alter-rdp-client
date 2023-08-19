namespace AlterApp.Services.Interfaces
{
    internal interface IAppSettingsService
    {
        public string AppName { get; }

        public string AppProjectWebsiteUri { get; }

        public string? GetAppVersion();

        public string? GetSemanticAppVersion();

        public string DefaultRemotePort { get; }
    }
}
