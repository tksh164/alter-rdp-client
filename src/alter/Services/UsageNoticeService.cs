using AlterApp.Services.Interfaces;
using AlterApp.Views;

namespace AlterApp.Services
{
    internal class UsageNoticeService : IUsageNoticeService
    {
        private readonly UsageWindow _usageWindow;

        public UsageNoticeService(UsageWindow usageWindow)
        {
            _usageWindow = usageWindow;
        }

        public void ShowUsage()
        {
            _ = _usageWindow.ShowDialog();
        }
    }
}
