using BookStore_Management_AppDesktop.Models.DTOs.CustomerDTOs;
using BookStore_Management_AppDesktop.Services;
using BookStore_Management_AppDesktop.Services.API.CustomerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DocumentFormat.OpenXml.Wordprocessing;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class AddCustomerViewModel : ObservableValidator
    {
        private readonly ICustomerApiService _apiService;
        private readonly IDialogService _dialogService;

        [ObservableProperty]
        [Required(ErrorMessage = "Customer name is required.")]
        private string _name = string.Empty;

        [ObservableProperty]
        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^(0[3|5|7|8|9])+([0-9]{8})$", ErrorMessage = "Invalid phone number.")]
        private string _phone = string.Empty;

        [ObservableProperty]
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _address = string.Empty;

        public AddCustomerViewModel(ICustomerApiService apiService, IDialogService dialogService)
        {
            _apiService = apiService;
            _dialogService = dialogService;
        }

        [RelayCommand]
        public async Task<CustomerResponseDto?> SaveAsync()
        {
            ValidateAllProperties();
            if (HasErrors) return null;

            var dto = new CustomerCreateDto { Name = Name, Phone = Phone, Email = Email, Address = Address };
            var result = await _apiService.CreateCustomerAsync(dto);

            if (result == null) _dialogService.ShowMessage("Failed to add customer.");
            return result;
        }
    }
}