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
            string settingStoreFolderPath = GetSettingStoreFolderPath();
            string settingFilePath = Path.Combine(settingStoreFolderPath, AppConstants.SettingFileName);
            if (!File.Exists(settingFilePath))
            {
                InitializeSettingFile(settingFilePath);
            }
            return settingFilePath;
        }

        private static string GetSettingStoreFolderPath()
        {
            string settingStoreFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AppConstants.SettingStoreFolderName);
            if (!Directory.Exists(settingStoreFolderPath))
            {
                Directory.CreateDirectory(settingStoreFolderPath);
            }
            return settingStoreFolderPath;
        }

        private static void InitializeSettingFile(string settingFilePath)
        {
            string? appFolderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (string.IsNullOrEmpty(appFolderPath))
            {
                throw new FileNotFoundException("The app folder path was null or empty.", appFolderPath);
            }
            string templateFilePath = Path.Combine(appFolderPath, AppConstants.SettingFileTemplateFileName);
            File.Copy(templateFilePath, settingFilePath, true);
        }
    }
}
