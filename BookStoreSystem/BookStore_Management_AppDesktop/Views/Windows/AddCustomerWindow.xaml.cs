using BookStore_Management_AppDesktop.Models.DTOs.CustomerDTOs;
using BookStore_Management_AppDesktop.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Input;
using System.Windows;

namespace BookStore_Management_AppDesktop.Views.Windows
{
    public partial class AddCustomerWindow : Window
    {
        public AddCustomerViewModel ViewModel { get; }
        public CustomerResponseDto? CustomerResult { get; private set; }

        public AddCustomerWindow()
        {
            InitializeComponent();
            ViewModel = App.ServiceProvider!.GetRequiredService<AddCustomerViewModel>();
            DataContext = ViewModel;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await ViewModel.SaveAsync();
            if (result != null)
            {
                CustomerResult = result;
                DialogResult = true;
                Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

    }
}