using System.Windows;
using BookStore_Management_AppDesktop.Models.DTOs.CustomerDTOs;
using BookStore_Management_AppDesktop.Services;
using BookStore_Management_AppDesktop.Services.API.CustomerServices;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Input;

namespace BookStore_Management_AppDesktop.Views.Windows
{
    public partial class AddCustomerWindow : Window
    {
        private readonly ICustomerApiService _customerApiService;
        private readonly IDialogService _dialogService;
        public CustomerResponseDto? CustomerResult { get; private set; }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        public AddCustomerWindow()
        {
            InitializeComponent();
            _customerApiService = App.ServiceProvider!.GetRequiredService<ICustomerApiService>();
            _dialogService = App.ServiceProvider!.GetRequiredService<IDialogService>();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                _dialogService.ShowMessage("Please enter customer name.");
                return;
            }

            try
            {
                var customerDto = new CustomerCreateDto
                {
                    Name = NameTextBox.Text,
                    Phone = string.IsNullOrWhiteSpace(PhoneTextBox.Text) ? null : PhoneTextBox.Text,
                    Email = string.IsNullOrWhiteSpace(EmailTextBox.Text) ? null : EmailTextBox.Text,
                    Address = string.IsNullOrWhiteSpace(AddressTextBox.Text) ? null : AddressTextBox.Text
                };

                var result = await _customerApiService.CreateCustomerAsync(customerDto);
                if (result != null)
                {
                    _dialogService.ShowMessage($"Customer {result.Name} added successfully!");
                    CustomerResult = result; // Gán k?t qu? vào ?ây
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    _dialogService.ShowMessage("Failed to add customer. Please try again.");
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessage($"Error: {ex.Message}");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}