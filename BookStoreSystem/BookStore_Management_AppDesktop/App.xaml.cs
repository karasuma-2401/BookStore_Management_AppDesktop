using BookStore_Management_AppDesktop.Services;
using BookStore_Management_AppDesktop.Services.API;
using BookStore_Management_AppDesktop.Services.API.BookServices; 
using BookStore_Management_AppDesktop.Services.API.CartServices;
using BookStore_Management_AppDesktop.Services.API.CategoryServices;
using BookStore_Management_AppDesktop.Services.API.CustomerServices;
using BookStore_Management_AppDesktop.Services.API.EmployeeServices;
using BookStore_Management_AppDesktop.Services.API.Import;
using BookStore_Management_AppDesktop.Services.API.InvoiceServices;
using BookStore_Management_AppDesktop.Services.API.ReportServices;
using BookStore_Management_AppDesktop.Services.Export;
using BookStore_Management_AppDesktop.Services.Navigation;
using BookStore_Management_AppDesktop.Services.Realtime;
using BookStore_Management_AppDesktop.ViewModels;
using BookStore_Management_AppDesktop.Views.Pages;
using BookStore_Management_AppDesktop.Views.Pages.BookViews; 
using BookStore_Management_AppDesktop.Views.Pages.InvoiceViews;
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
            services.AddSingleton<ICartService, CartService>();
            services.AddSingleton<IBookHubService, BookHubService>();

            services.AddSingleton<IBookApiService, BookApiService>();
            services.AddSingleton<IAuthorApiService, AuthorApiService>();
            services.AddSingleton<IEmployeeApiService, EmployeeApiService>();
            services.AddSingleton<IImportApiService, ImportApiService>();
            services.AddSingleton<ICustomerApiService, CustomerApiService>();
            services.AddSingleton<Wpf.Ui.IContentDialogService, Wpf.Ui.ContentDialogService>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<IReportApiService, ReportApiService>();
            services.AddSingleton<IExportService, ExportService>();

            services.AddSingleton<IInvoiceApiService, InvoiceApiService>();
            services.AddSingleton<IInvoiceExportService, InvoiceExportService>(); 
            services.AddSingleton<IVoucherApiService, VoucherApiService>();
            services.AddSingleton<ICategoryApiService, CategoryApiService>();

            services.AddSingleton<IEmployeeApiService, EmployeeApiService>();
            services.AddSingleton<IEmployeeShiftApiService, EmployeeShiftApiService>();
            

            // // Get ViewModels
            services.AddTransient<MainViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<InventoryViewModel>();
            services.AddTransient<BookFormViewModel>();
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<AuthorSelectionViewModel>();
            services.AddTransient<BookViewModel>();
            services.AddTransient<EmployeeViewModel>();
            services.AddTransient<CustomerViewModel>();
            services.AddTransient<ImportCreateViewModel>(); 
            services.AddTransient<ImportHistoryViewModel>();
            services.AddTransient<InvoiceViewModel>();
            services.AddTransient<InvoiceDetailViewModel>();
            services.AddTransient<ReportViewModel>();
            services.AddTransient<SaleCartViewModel>();
            services.AddTransient<CategorySelectionViewModel>();
            services.AddTransient<ShiftScheduleViewModel>();
            services.AddTransient<PayrollViewModel>();
            services.AddTransient<AbsenceManagementViewModel>();
            services.AddTransient<KioskCheckInViewModel>();


            // // Get View 
            services.AddTransient<MainWindow>();
            services.AddTransient<LoginWindow>();
            services.AddTransient<AddBookWindow>(); 
            services.AddTransient<EditBookWindow>();
            services.AddTransient<BookPage>();
            services.AddTransient<SaleCartPage>();
            services.AddTransient<InventoryPage>();
            services.AddTransient<SettingsPage>();
            services.AddTransient<EmployeePage>();
            services.AddTransient<CustomerPage>();
            services.AddTransient<ImportHistoryPage>(); 
            services.AddTransient<CreateImportPage>(); 
            services.AddTransient<InvoicePage>();
            services.AddTransient<InvoiceDetailPage>();
            services.AddTransient<ReportPage>();
            services.AddTransient<ShiftSchedulePage>();
            services.AddTransient<PayrollPage>();
            services.AddTransient<AbsenceManagementPage>();
            services.AddTransient<KioskCheckInPage>();
        }
    }

}
