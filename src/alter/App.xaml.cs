using System;
using System.Runtime.ExceptionServices;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using AlterApp.Services;
using AlterApp.Services.Interfaces;
using AlterApp.ViewModels;
using AlterApp.Views;
using AlterApp.Models;

namespace AlterApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public sealed partial class App : Application
    {
        public App()
        {
            // NOTE: For handling exceptions that were thrown before completing configuring services.
            AppDomain.CurrentDomain.FirstChanceException += FirstChanceExceptionHandler;

            Services = ConfigureServices();
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;

            // NOTE: Unregister the handler after complete service configuration.
            AppDomain.CurrentDomain.FirstChanceException -= FirstChanceExceptionHandler;
        }

        private static void FirstChanceExceptionHandler(object? sender, FirstChanceExceptionEventArgs e)
        {
            string message = "A catastrophic error occurred.\n" +
                "Message: " + e.Exception.Message + "\n" +
                "StackTrace:\n" + e.Exception.StackTrace;
            MessageBox.Show(message, "Catastrophic Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Environment.Exit(AppExitCodes.CatastrophicError);
        }

        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            var reportService = App.Current.Services.GetService<IUnhandledExceptionReportService>();
            if (reportService == null) Environment.Exit(AppExitCodes.UnhandledException1);
            reportService.ReportUnhandledException(e.ExceptionObject as Exception);
        }

        /// <summary>
        /// Gets the current <see cref="App"/> instance in use.
        /// </summary>
        public new static App Current => (App)Application.Current;

        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
        /// </summary>
        public IServiceProvider Services { get; }

        /// <summary>
        /// Configure the services for the application.
        /// </summary>
        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Services
            services.AddTransient<IUnhandledExceptionReportService, UnhandledExceptionReportService>();
            services.AddSingleton<IAppSettingsService, AppSettingsService>();
            services.AddTransient<CommandLineArgsService>();
            services.AddTransient<IExceptionReportWindowViewModelService, ExceptionReportWindowViewModelService>();
            services.AddTransient<IMainWindowViewModelService, MainWindowViewModelService>();

            // ViewModels
            services.AddTransient<ExceptionReportWindowViewModel>();
            services.AddTransient<MainWindowViewModel>();
            services.AddTransient<UsageWindowViewModel>();

            // Views
            services.AddTransient<ExceptionReportWindow>();
            services.AddTransient<UsageWindow>();

            return services.BuildServiceProvider();
        }
    }
}
