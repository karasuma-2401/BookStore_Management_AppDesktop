using BookStore_Management_AppDesktop.ViewModels.BooksViewModel;
using System.Windows;

namespace BookStore_Management_AppDesktop.Views.Windows
{
    public partial class CategoryManagementWindow : Window
    {
        public CategoryManagementWindow(CategoryManagementViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is CategoryManagementViewModel viewModel)
            {
                await viewModel.LoadDataAsync();
            }
        }
    }
}