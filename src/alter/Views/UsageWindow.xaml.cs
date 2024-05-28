using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using AlterApp.Views.Interfaces;
using AlterApp.ViewModels;

namespace AlterApp.Views
{
    /// <summary>
    /// Interaction logic for UsageWindow.xaml
    /// </summary>
    public partial class UsageWindow : Window, IClosable
    {
        public UsageWindow()
        {
            InitializeComponent();
            DataContext = DataContext = App.Current.Services.GetService<UsageWindowViewModel>();
        }
    }
}
