using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using AlterApp.ViewModels;
using AlterApp.Views.Interfaces;

namespace AlterApp.Views
{
    /// <summary>
    /// Interaction logic for UsageWindow.xaml
    /// </summary>
    public partial class UsageWindow : Window, IUsageWindow, IClosable
    {
        public UsageWindow()
        {
            InitializeComponent();
            DataContext = DataContext = App.Current.Services.GetService<UsageWindowViewModel>();
        }
    }
}
