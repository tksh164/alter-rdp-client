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
                            {AppConstants.AppSettingJsonColumnName}->>'{jsonPath}' AS {AppConstants.AppSettingQueryResultColumnName}
                        FROM
                            {AppConstants.AppSettingTableName}
                        WHERE ROWID = {AppConstants.AppSettingRowId};
                        ";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return reader.GetFieldValue<T>(AppConstants.AppSettingQueryResultColumnName);
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
                        UPDATE {AppConstants.AppSettingTableName}
                        SET {AppConstants.AppSettingJsonColumnName} = json_set({AppConstants.AppSettingJsonColumnName}, '{jsonPath}', {newValue})
                        WHERE ROWID = {AppConstants.AppSettingRowId};
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
