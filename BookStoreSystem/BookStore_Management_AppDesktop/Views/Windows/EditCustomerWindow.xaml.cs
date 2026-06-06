using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BookStore_Management_AppDesktop.Models.DTOs.CustomerDTOs;
using BookStore_Management_AppDesktop.Services;
using BookStore_Management_AppDesktop.Services.API.CustomerServices;
using BookStore_Management_AppDesktop.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore_Management_AppDesktop.Views.Windows
{
    public partial class EditCustomerWindow : Window
    {
        // Sử dụng ViewModel thay cho các biến API thủ công
        public EditCustomerViewModel ViewModel { get; }

        public EditCustomerWindow(CustomerResponseDto customer)
        {
            InitializeComponent();

            var apiService = App.ServiceProvider!.GetRequiredService<ICustomerApiService>();
            var dialogService = App.ServiceProvider!.GetRequiredService<IDialogService>();

            ViewModel = new EditCustomerViewModel(apiService, dialogService, customer);
            DataContext = ViewModel;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            bool isSuccess = await ViewModel.UpdateAsync();

            if (isSuccess)
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
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