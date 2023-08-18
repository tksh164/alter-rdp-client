namespace AlterApp.Services.Interfaces
{
    internal interface IExceptionReportWindowViewModelService
    {
        public string GetWindowTitle();
        public void OpenProjectWebsite();
        public void TerminateApplicationWithErrorExitCode();
    }
}
