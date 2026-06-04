using BookStore_Management_AppDesktop.ViewModels.BooksViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BookStore_Management_AppDesktop.Views.Windows
{
    public partial class AuthorManagementWindow : Window
    {
        public AuthorManagementWindow(AuthorManagementViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is AuthorManagementViewModel viewModel)
            {
                await viewModel.LoadDataAsync();
            }
        }
    }
}
