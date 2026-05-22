using System.Windows;
using BookStore_Management_AppDesktop.Models.DTOs.CustomerDTOs;
using BookStore_Management_AppDesktop.Services;
using BookStore_Management_AppDesktop.Services.API.CustomerServices;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore_Management_AppDesktop.Views.Windows
{
    public partial class EditCustomerWindow : Window
    {
        private readonly ICustomerApiService _customerApiService;
        private readonly IDialogService _dialogService;
        private CustomerResponseDto? _customer;

        public EditCustomerWindow(CustomerResponseDto customer)
        {
            InitializeComponent();
            _customerApiService = App.ServiceProvider!.GetRequiredService<ICustomerApiService>();
            _dialogService = App.ServiceProvider!.GetRequiredService<IDialogService>();
            _customer = customer;
            LoadCustomerData();
        }

        private void LoadCustomerData()
        {
            if (_customer != null)
            {
                IdTextBox.Text = _customer.CustomerId.ToString();
                NameTextBox.Text = _customer.Name;
                PhoneTextBox.Text = _customer.Phone ?? string.Empty;
                EmailTextBox.Text = _customer.Email ?? string.Empty;
                AddressTextBox.Text = _customer.Address ?? string.Empty;
                DebtTextBox.Text = _customer.Debt.ToString("N0") + " ₫";
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                _dialogService.ShowMessage("Please enter customer name.");
                return;
            }

            if (_customer == null)
                return;

            try
            {
                var customerDto = new CustomerUpdateDto
                {
                    Name = NameTextBox.Text,
                    Phone = string.IsNullOrWhiteSpace(PhoneTextBox.Text) ? null : PhoneTextBox.Text,
                    Email = string.IsNullOrWhiteSpace(EmailTextBox.Text) ? null : EmailTextBox.Text,
                    Address = string.IsNullOrWhiteSpace(AddressTextBox.Text) ? null : AddressTextBox.Text
                };

                var result = await _customerApiService.UpdateCustomerAsync(_customer.CustomerId, customerDto);
                if (result)
                {
                    _dialogService.ShowMessage($"Customer {NameTextBox.Text} updated successfully!");
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    _dialogService.ShowMessage("Failed to update customer. Please try again.");
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
