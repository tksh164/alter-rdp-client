namespace AlterApp.Services.Interfaces
{
    internal interface IExceptionReportWindowViewModelService
    {
        public string GetWindowTitle();

        public void OpenIssueReportUri();

        public void TerminateApplicationWithErrorExitCode();
    }
}
