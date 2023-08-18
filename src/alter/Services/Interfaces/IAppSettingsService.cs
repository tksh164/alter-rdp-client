namespace AlterApp.Services.Interfaces
{
    internal interface IAppSettingsService
    {
        public string AppName { get; }

        public string GetRemotePort();
    }
}
