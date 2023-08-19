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

        public void OpenIssueReportUri()
        {
            string issueReportUri = _appSettingsService.AppProjectWebsiteUri + "/issues";
            Process.Start(new ProcessStartInfo()
            {
                FileName = issueReportUri,
                UseShellExecute = true,
            });
        }

        public void TerminateApplicationWithErrorExitCode()
        {
            Environment.Exit(AppExitCode.ViaCrashReporter);
        }
    }
}
