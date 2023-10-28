namespace AlterApp.Models
{
    internal static class AppConstants
    {
        public static string AppName = "Alter";
        public static string ProjectWebsiteUri = "https://github.com/tksh164/alter-rdp-client";

        public static string SettingFileName = "setting.db";
        public static string AppSettingTableName = "app_settings";
        public static string AppSettingJsonColumnName = "json";
        public static uint AppSettingRowId = 1;
        public static string AppSettingQueryResultColumnName = "value";

        public static double MainWindowMinWidth = 780;
        public static double MainWindowMinHeight = 520;

        public static double DefaultMainWindowWidth = 1024;
        public static double DefaultMainWindowHeight = 768;

        public static uint DefaultRdpPort = 3389;
    }
}
