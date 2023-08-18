using System;

namespace AlterApp.Services.Interfaces
{
    internal interface IUnhandledExceptionReportService
    {
        public void ReportUnhandledException(Exception? ex);
    }
}
