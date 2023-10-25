using System;
using System.IO;
using System.Reflection;
using System.Data;
using Microsoft.Data.Sqlite;
using AlterApp.Services.Interfaces;
using AlterApp.Models;

namespace AlterApp.Services
{
    internal class AppSettingsService : IAppSettingsService
    {
        public AppSettingsService()
        {
        }

        public string AppName => AppConstants.AppName;

        public string AppProjectWebsiteUri => AppConstants.ProjectWebsiteUri;

        public string? GetAppVersion()
        {
            return (((Assembly.GetEntryAssembly())?.GetName())?.Version)?.ToString();
        }

        public string? GetAppVersionSemanticPart()
        {
            string? appVersion = GetAppVersion();
            return appVersion?[..appVersion.LastIndexOf(".", StringComparison.OrdinalIgnoreCase)];
        }

        public T GetSettingValue<T>(string name, T defaultValue)
        {
            return ReadAppSettingValue<T>(name, defaultValue);
        }

        private static T ReadAppSettingValue<T>(string settingName, T defaultValue)
        {
            string jsonPath = "$." + settingName;
            string connectionString = GetSettingDbConnectionString();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText =
                        $@"
                        SELECT
                            json->>'{jsonPath}' AS SettingValue
                        FROM
                            app_settings
                        ORDER BY ROWID ASC
                        LIMIT 1;
                        ";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return reader.GetFieldValue<T>("SettingValue");
                        }
                    }
                }
            }
            return defaultValue;
        }

        private static string GetSettingDbConnectionString()
        {
            string appSettingFilePath = GetSettingFilePath();
            return string.Format("Data Source={0}", appSettingFilePath);
        }

        private static string GetSettingFilePath()
        {
            string appDirectoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            return Path.Combine(appDirectoryPath, AppConstants.SettingFileName);
        }
    }
}
