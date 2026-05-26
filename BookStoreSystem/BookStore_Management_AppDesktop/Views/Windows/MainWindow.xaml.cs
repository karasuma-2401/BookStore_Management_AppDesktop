using BookStore_Management_AppDesktop.Helpers.Enums;
using BookStore_Management_AppDesktop.Services.Navigation;
using BookStore_Management_AppDesktop.Services.Realtime;
using BookStore_Management_AppDesktop.ViewModels;
using System.Windows;
using System.ComponentModel;

namespace BookStore_Management_AppDesktop.Views.Windows
{

    public partial class MainWindow : Window
    {
        private readonly IBookHubService _bookHubService;

        public MainWindow(
            MainViewModel viewModel, INavigationService navigationService,
            Wpf.Ui.IContentDialogService contentDialogService,
            IBookHubService bookHubService) 
        {
            InitializeComponent();

            this.DataContext = viewModel;
            _bookHubService = bookHubService;

            navigationService.SetFrame(RootFrame);

            contentDialogService.SetDialogHost(RootContentDialogPresenter);


            navigationService.NavigateTo(PageType.Books);

            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;
        }

        private async void MainWindow_Loaded(object? sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("[SignalR] MainWindow loaded. Starting connection...");
            await _bookHubService.StartAsync();
        }

        private async void MainWindow_Closing(object? sender, CancelEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("[SignalR] MainWindow closing. Stopping connection...");
            await _bookHubService.StopAsync();
        }
    }
}