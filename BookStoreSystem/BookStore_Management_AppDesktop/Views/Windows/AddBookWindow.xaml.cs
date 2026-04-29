using BookStore_Management_AppDesktop.Services;
using BookStore_Management_AppDesktop.Services.API;
using BookStore_Management_AppDesktop.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Input;

namespace BookStore_Management_AppDesktop.Views.Windows
{
    public partial class AddBookWindow : Window
    {
        private readonly BookFormViewModel _viewModel;

        public AddBookWindow()
        {
            InitializeComponent();

            _viewModel = App.ServiceProvider!.GetRequiredService<BookFormViewModel>();

            var dialogService = App.ServiceProvider!.GetRequiredService<IDialogService>();

            _viewModel.OnShowMessage = (message) => dialogService.ShowMessage(message);

            _viewModel.OnRequestClose = () => this.Close();

            this.DataContext = _viewModel;
            this.Loaded += AddBookWindow_Loaded;
        }

        private async void AddBookWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.InitializeAsync();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
    }
}