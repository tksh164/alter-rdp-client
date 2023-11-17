using System;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;
using AlterApp.Services.Interfaces;
using AlterApp.ViewModels;
using AlterApp.Views;
using AlterApp.Models;

namespace AlterApp.Services
{
    internal class UnhandledExceptionReportService : IUnhandledExceptionReportService
    {
        private readonly ExceptionReportWindow _window;

        public UnhandledExceptionReportService(ExceptionReportWindow window)
        {
            _window = window;
        }

        public void ReportUnhandledException(Exception? ex)
        {
            var viewModel = (ExceptionReportWindowViewModel)_window.DataContext;
            viewModel.ReportContentText = BuildExceptionReportText(ex);
            _window.Closed += viewModel.OnWindowClosed;
            _window.ShowDialog();
        }

        private string BuildExceptionReportText(Exception? exception)
        {
            if (exception == null)
            {
                return "The exception is null.";
            }

            var reportText = new StringBuilder();
            reportText.AppendLine(@"**** ENVIRONMENT ****");

            // App
            string appVersion = AppConstants.GetAppVersion() ?? "(Could not get app version)";
            string processArchitecture = RuntimeInformation.ProcessArchitecture.ToString();
            string dotNet = RuntimeInformation.FrameworkDescription;
            reportText.AppendFormat(@"App version: {0}", appVersion);
            reportText.AppendLine();
            reportText.AppendFormat(@"Process architecture: {0}", processArchitecture);
            reportText.AppendLine();
            reportText.AppendFormat(@".NET: {0}", dotNet);
            reportText.AppendLine();

            // OS
            string productName = GetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName", "n/a");
            OperatingSystem os = Environment.OSVersion;
            string ubr = GetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "UBR", "0");
            string displayVersion = GetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "DisplayVersion", "n/a");
            string releaseId = GetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ReleaseId", "n/a");
            string currentVersion = GetRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentVersion", "n/a");
            string osArchitecture = RuntimeInformation.OSArchitecture.ToString();
            reportText.AppendFormat(@"OS: {0} {1}.{2}.{3}.{4} ({5}, {6}, {7}) {8}", productName, os.Version.Major, os.Version.Minor, os.Version.Build, ubr, displayVersion, releaseId, currentVersion, osArchitecture);
            reportText.AppendLine();

            // Stack trace
            int nestLevel = 0;
            var ex = exception;
            while (ex != null)
            {
                reportText.AppendLine();
                reportText.AppendFormat(@"**** EXCEPTION (Level:{0}) ****", nestLevel);
                reportText.AppendLine();
                reportText.AppendLine(ex.Message);
                reportText.AppendFormat(@"Exception: {0}", ex.GetType().FullName);
                reportText.AppendLine();
                reportText.AppendFormat(@"HResult: 0x{0:x8} ({0})", ex.HResult);
                reportText.AppendLine();
                reportText.AppendLine(@"**** STACK TRACE ****");
                reportText.AppendLine(ex.StackTrace);
                ex = ex.InnerException;
                nestLevel++;
            }

            return reportText.ToString();
        }

        private static string GetRegistryValue(string keyName, string valueName, string defaultValue)
        {
            try
            {
                #pragma warning disable CA1416 // Validate platform compatibility
                return ((string?)Registry.GetValue(keyName, valueName, defaultValue)) ?? defaultValue;
                #pragma warning restore CA1416 // Validate platform compatibility
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
