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
        private bool _isDataLoaded = false;
        public InventoryPage()
        {
            InitializeComponent();

            var viewModel = new InventoryViewModel();

            viewModel.OnShowMessage = (message) =>
            {
                var msgBox = new CustomMessageBox(message);
                msgBox.ShowDialog();
            };

            viewModel.OnRequestConfirm = async (title, content) =>
            {
                var confirmDialog = new Wpf.Ui.Controls.MessageBox
                {
                    Title = title,
                    Content = content,
                    PrimaryButtonText = "Xóa sách",
                    CloseButtonText = "Hủy",
                    PrimaryButtonAppearance = Wpf.Ui.Controls.ControlAppearance.Danger
                };

                var result = await confirmDialog.ShowDialogAsync();

                return result == Wpf.Ui.Controls.MessageBoxResult.Primary;
            };

            this.DataContext = viewModel;
        }


        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isDataLoaded) return;

            if (this.DataContext is InventoryViewModel viewModel)
            {
                await viewModel.LoadDataAsync();
                _isDataLoaded = true; 
            }
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
