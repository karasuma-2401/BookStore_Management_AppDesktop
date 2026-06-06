using System.ComponentModel.DataAnnotations;
using BookStore_Management_AppDesktop.Models.DTOs.CustomerDTOs;
using BookStore_Management_AppDesktop.Services;
using BookStore_Management_AppDesktop.Services.API.CustomerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class EditCustomerViewModel : ObservableValidator
    {
        private readonly ICustomerApiService _apiService;
        private readonly IDialogService _dialogService;
        private readonly int _customerId;

        [ObservableProperty] private int _id;
        [ObservableProperty] private string _debt = string.Empty;

        [ObservableProperty]
        [Required(ErrorMessage = "Name is required.")]
        private string _name = string.Empty;

        [ObservableProperty]
        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^(0[3|5|7|8|9])+([0-9]{8})$", ErrorMessage = "Invalid phone.")]
        private string _phone = string.Empty;

        [ObservableProperty]
        [EmailAddress(ErrorMessage = "Invalid email.")]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _address = string.Empty;

        public EditCustomerViewModel(ICustomerApiService apiService, IDialogService dialogService, CustomerResponseDto customer)
        {
            _apiService = apiService;
            _dialogService = dialogService;
            _customerId = customer.CustomerId;

            Id = customer.CustomerId;
            Name = customer.Name;
            Phone = customer.Phone ?? "";
            Email = customer.Email ?? "";
            Address = customer.Address ?? "";
            Debt = customer.Debt.ToString("N0") + " ₫";
        }

        [RelayCommand]
        public async Task<bool> UpdateAsync()
        {
            ValidateAllProperties();
            if (HasErrors) return false;

            var dto = new CustomerUpdateDto { Name = Name, Phone = Phone, Email = Email, Address = Address };
            var result = await _apiService.UpdateCustomerAsync(_customerId, dto);

            if (!result) _dialogService.ShowMessage("Failed to update customer.");
            return result;
        }
    }
}