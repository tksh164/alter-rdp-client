using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using AlterApp.ViewModels;

namespace AlterApp.Views
{
    /// <summary>
    /// Interaction logic for ExceptionReportWindow.xaml
    /// </summary>
    public partial class ExceptionReportWindow : Window
    {
        public ExceptionReportWindow()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService<ExceptionReportWindowViewModel>();
        }
    }
}
