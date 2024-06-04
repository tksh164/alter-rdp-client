using System;
using System.Reflection;
using System.Diagnostics;

namespace AlterApp.Models
{
    internal static class AppConstants
    {
        public static string AppName = "Alter";

        public static string? GetAppVersion()
        {
            return (((Assembly.GetEntryAssembly())?.GetName())?.Version)?.ToString();
        }

        public static string? GetAppVersionSemanticPart()
        {
            var appVersion = GetAppVersion();
            return appVersion?[..appVersion.LastIndexOf(".", StringComparison.OrdinalIgnoreCase)];
        }

        public static string? GetCommitHash()
        {
            var assembly = Assembly.GetEntryAssembly();
            if (assembly == null) return null;
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            var productVersion = fileVersionInfo.ProductVersion;
            if (productVersion == null) return null;
            var separatorIndex = productVersion.LastIndexOf("+", StringComparison.OrdinalIgnoreCase);
            if (separatorIndex < 0) return null;
            return productVersion[(separatorIndex + 1)..];
        }

        public static string ProjectWebsiteUri = "https://github.com/tksh164/alter-rdp-client";

        public static string SettingFileTemplateFileName = "setting.template";
        public static string SettingStoreFolderName = "AlterRDClient";
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
