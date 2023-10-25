using System;
using System.Diagnostics;
using AlterApp.Services.Interfaces;
using AlterApp.Models;

namespace AlterApp.Services
{
    internal class ExceptionReportWindowViewModelService : IExceptionReportWindowViewModelService
    {
        public ExceptionReportWindowViewModelService()
        {
        }

        public string GetWindowTitle()
        {
            return AppConstants.AppName;
        }

        public void OpenIssueReportUri()
        {
            string issueReportUri = AppConstants.ProjectWebsiteUri + "/issues";
            Process.Start(new ProcessStartInfo()
            {
                FileName = issueReportUri,
                UseShellExecute = true,
            });
        }

        public void TerminateApplicationWithErrorExitCode()
        {
            Environment.Exit(AppExitCodes.ViaCrashReporter);
        }
    }
}
