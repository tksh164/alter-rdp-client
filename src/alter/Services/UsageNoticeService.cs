using Microsoft.Extensions.DependencyInjection;
using AlterApp.Services.Interfaces;
using AlterApp.Views.Interfaces;

namespace AlterApp.Services
{
    internal class UsageNoticeService : IUsageNoticeService
    {
        public UsageNoticeService()
        {
        }

        public void ShowUsage()
        {
            IUsageWindow? usageWindow = App.Current.Services.GetService<IUsageWindow>();
            _ = usageWindow?.ShowDialog();
        }
    }
}
