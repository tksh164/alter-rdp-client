using System;
using System.Diagnostics;
using AlterApp.Services.Interfaces;

namespace AlterApp.Services
{
    internal class ExceptionReportWindowViewModelService : IExceptionReportWindowViewModelService
    {
        private readonly IAppSettingsService _appSettingsService;

        public ExceptionReportWindowViewModelService(IAppSettingsService appSettingsService)
        {
            _appSettingsService = appSettingsService;
        }

        public string GetWindowTitle()
        {
            return _appSettingsService.AppName;
        }

        public void OpenProjectWebsite()
        {
            const string projectWebsiteUri = "https://github.com/tksh164/alter-rdp-client/issues";
            Process.Start(new ProcessStartInfo()
            {
                FileName = projectWebsiteUri,
                UseShellExecute = true,
            });
        }

        public void TerminateApplicationWithErrorExitCode()
        {
            Environment.Exit(AppExitCode.ViaCrashReporter);
        }
    }
}
