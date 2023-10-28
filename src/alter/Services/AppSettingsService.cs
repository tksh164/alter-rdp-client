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
            return ReadAppSettingValue(name, defaultValue);
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
                            json->>'{jsonPath}' AS value
                        FROM
                            app_settings
                        WHERE ROWID = 1;
                        ";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return reader.GetFieldValue<T>("value");
                        }
                    }
                }
            }
            return defaultValue;
        }

        public int SetSettingValue<T>(string name, T newValue)
        {
            return WriteAppSettingValue(name, newValue);
        }

        private static int WriteAppSettingValue<T>(string settingName, T newValue)
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
                        UPDATE app_settings
                        SET json = json_set(json, '{jsonPath}', {newValue})
                        WHERE ROWID = 1
                        ";
                    return command.ExecuteNonQuery();
                }
            }
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
