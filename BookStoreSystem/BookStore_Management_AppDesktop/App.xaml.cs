using BookStore_Management_AppDesktop.Services;
using BookStore_Management_AppDesktop.Services.API;
using BookStore_Management_AppDesktop.Services.Navigation;
using BookStore_Management_AppDesktop.ViewModels;
using BookStore_Management_AppDesktop.Views.Pages;
using BookStore_Management_AppDesktop.Views.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Windows;

namespace BookStore_Management_AppDesktop
{
    public partial class App : Application
    {
        public static IServiceProvider? ServiceProvider { get; private set; }

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
            services.AddSingleton<IConfiguration>(provider =>
            {
                return new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();
            });

            // // Get Services (Connect BE, Navigate)
            services.AddHttpClient<IAuthService, AuthService>();
            services.AddSingleton<CloudinaryService>();
            services.AddSingleton<INavigationService, NavigationService>();

            services.AddSingleton<IBookApiService, BookApiService>(); 
            services.AddSingleton<IAuthorApiService, AuthorApiService>(); 

            // // Get ViewModels
            services.AddTransient<MainViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<InventoryViewModel>();
            services.AddTransient<BookFormViewModel>();
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<AuthorSelectionViewModel>();
            services.AddTransient<BookViewModel>();


            // // Get View 
            services.AddTransient<MainWindow>();
            services.AddTransient<LoginWindow>();
            services.AddTransient<AddBookWindow>(); 
            services.AddTransient<EditBookWindow>();
            services.AddTransient<BookPage>();
            services.AddTransient<InventoryPage>();
            services.AddTransient<SettingsPage>();
        }
    }

}
