using BookStore_Management_AppDesktop.Models;
using BookStore_Management_AppDesktop.Services;
using BookStore_Management_AppDesktop.Services.API;
using BookStore_Management_AppDesktop.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Input;

namespace BookStore_Management_AppDesktop.Views.Windows
{
    /// <summary>
    /// Interaction logic for EditBookWindow.xaml
    /// </summary>
    public partial class EditBookWindow : Window
    {
        private readonly BookFormViewModel _viewModel;

        public EditBookWindow(Book bookToEdit)
        {
            InitializeComponent();

            _viewModel = App.ServiceProvider!.GetRequiredService<BookFormViewModel>();

            _viewModel.SetupEditMode(bookToEdit);

            _viewModel.OnShowMessage = (message) =>
            {
                var msgBox = new CustomMessageBox(message);
                msgBox.ShowDialog();
            };

            _viewModel.OnRequestClose = () => this.Close();

            this.DataContext = _viewModel;
            this.Loaded += EditBookWindow_Loaded;
        }

        private async void EditBookWindow_Loaded(object sender, RoutedEventArgs e)
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