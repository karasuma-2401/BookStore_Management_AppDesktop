using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using BookStore_Management_AppDesktop.ViewModels;
using BookStore_Management_AppDesktop.Views.Windows;
using BookStore_Management_AppDesktop.Services.Navigation;
using BookStore_Management_AppDesktop.Services;

namespace BookStore_Management_AppDesktop
{
    public partial class App : Application
    {
        public IServiceProvider? ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var serviceCollection = new ServiceCollection();

            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            var LoginWindow = ServiceProvider.GetRequiredService<LoginWindow>();
            LoginWindow.Show();

            //var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            //mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // // Get Services (Connect BE, Navigate)
            // services.AddSingleton<IApiClient, ApiClient>();
            services.AddSingleton<INavigationService, NavigationService>();

            // // Get ViewModels
            services.AddTransient<MainViewModel>();
            services.AddTransient<LoginViewModel>();

            // // Get View 
            services.AddTransient<MainWindow>();
            services.AddTransient<LoginWindow>();
        }
    }

}
