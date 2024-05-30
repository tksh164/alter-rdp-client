using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using AlterApp.ViewModels.Interfaces;
using AlterApp.ViewModels;

namespace AlterApp.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService<MainWindowViewModel>();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            IWindowClosing? context = this.DataContext as IWindowClosing;
            if (context != null)
            {
                e.Cancel = context.OnClosing();
            }
        }

        private void Window_ContentRendered(object sender, System.EventArgs e)
        {
            if (DataContext is IWindowContentRendered dataContext)
            {
                dataContext.OnContentRendered();
            }
        }
    }
}
