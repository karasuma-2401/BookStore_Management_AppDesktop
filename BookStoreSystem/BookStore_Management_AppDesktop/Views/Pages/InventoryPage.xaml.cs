using BookStore_Management_AppDesktop.ViewModels;
using BookStore_Management_AppDesktop.Views.Windows;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace BookStore_Management_AppDesktop.Views.Pages
{

    public partial class InventoryPage : Page
    {
        public InventoryPage()
        {
            InitializeComponent();
            this.DataContext = new InventoryViewModel();
        }

        private async void btnAddBook_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddBookWindow();
            addWindow.ShowDialog();

            if (this.DataContext is InventoryViewModel viewModel)
            {
                await viewModel.LoadDataAsync();
            }
        }
    }
}
