
using BookStore_Management_AppDesktop.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace BookStore_Management_AppDesktop.Views.Pages
{
    /// <summary>
    /// Interaction logic for BookPage.xaml
    /// </summary>
    public partial class BookPage : Page
    {
        private bool _isDataLoaded = false;
        public BookPage()
        {
            InitializeComponent();

            this.DataContext = App.ServiceProvider!.GetRequiredService<BookViewModel>();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isDataLoaded) return;

            if (this.DataContext is BookViewModel viewModel)
            {
                await viewModel.LoadDataAsync();
                _isDataLoaded = true;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
