using BookStore_Management_AppDesktop.Models;
using BookStore_Management_AppDesktop.Services.API;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class UpdateEmployeeViewModel : ObservableObject
    {
        private readonly IEmployeeApiService _apiService;

        // Internal IDs for API calls
        private readonly int _employeeId;
        private readonly int _userId;

        // Flat properties for UI Binding
        [ObservableProperty] private int _displayEmployeeId;
        [ObservableProperty] private string _fullName = string.Empty;
        [ObservableProperty] private int _age;
        [ObservableProperty] private string _phone = string.Empty;
        [ObservableProperty] private string _address = string.Empty;
        [ObservableProperty] private decimal _salary;
        [ObservableProperty] private bool _isLoading = false;

        // Communication Actions
        public Action<string, string>? OnShowMessage { get; set; }
        public Action<bool>? OnRequestClose { get; set; }

        public UpdateEmployeeViewModel(Employee employee)
        {
            _apiService = new EmployeeApiService();

            // Mapping original data to flat properties
            _employeeId = employee.EmployeeId;
            _userId = employee.UserId;

            DisplayEmployeeId = employee.EmployeeId;
            FullName = employee.FullName ?? string.Empty;
            Age = employee.Age;
            Phone = employee.Phone ?? string.Empty;
            Address = employee.Address ?? string.Empty;
            Salary = employee.Salary;
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(FullName))
            {
                OnShowMessage?.Invoke("Validation Error", "Please enter the employee's full name.");
                return;
            }

            try
            {
                IsLoading = true;

                // Create a fresh object to send to the API
                var updatedEmployee = new Employee
                {
                    EmployeeId = _employeeId,
                    FullName = FullName,
                    Age = Age,
                    Phone = Phone,
                    Address = Address,
                    Salary = Salary,
                    UserId = _userId
                };

                bool isSuccess = await _apiService.UpdateEmployeeAsync(_employeeId, updatedEmployee);

                if (isSuccess)
                {
                    OnShowMessage?.Invoke("Success", "Employee information updated successfully!");
                    OnRequestClose?.Invoke(true);
                }
                else
                {
                    OnShowMessage?.Invoke("Error", "Update failed. Please check your data or connection.");
                }
            }
            catch (Exception ex)
            {
                OnShowMessage?.Invoke("System Error", $"An error occurred: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            OnRequestClose?.Invoke(false);
        }
    }
}