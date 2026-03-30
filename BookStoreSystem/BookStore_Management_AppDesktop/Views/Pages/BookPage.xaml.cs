
using BookStore_Management_AppDesktop.ViewModels;
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

            // Gán ViewModel giả để test giao diện Amazon lên hình
            this.DataContext = new BookViewModel();
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
