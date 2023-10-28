namespace AlterApp.Services.Interfaces
{
    internal interface IAppSettingsService
    {
        public string? GetAppVersion();

        public string? GetAppVersionSemanticPart();

        public T GetSettingValue<T>(string name, T defaultValue);

        public int SetSettingValue<T>(string name, T newValue);
    }
}
