using BookStore_Management_AppDesktop.Models.DTOs;
using BookStore_Management_AppDesktop.Services.API;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class AddUserViewModel : ObservableObject
    {
        private readonly IUserApiService _userApi;

        [ObservableProperty]
        private UserCreateDto _newUser = new()
        {
            FullName = string.Empty,
            Username = string.Empty,
            Email = string.Empty,
            Password = string.Empty,
            RoleId = "staff" // Default role to pass validation
        };

        [ObservableProperty]
        private string _password = string.Empty;

        public AddUserViewModel()
        {
            _userApi = new UserApiService();
        }

        [RelayCommand]
        private async Task CreateUser(Window window)
        {
            // Initial Client-side validation
            if (string.IsNullOrWhiteSpace(NewUser.Username) ||
                string.IsNullOrWhiteSpace(Password) ||
                string.IsNullOrWhiteSpace(NewUser.Email))
            {
                MessageBox.Show("Please fill in all required fields.", "Notification");
                return;
            }

            try
            {
                NewUser.Password = Password;

                // Receive the Tuple result
                var result = await _userApi.CreateUserAsync(NewUser);

                if (result.IsSuccess)
                {
                    MessageBox.Show(result.Message, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    if (window != null)
                    {
                        window.DialogResult = true;
                        window.Close();
                    }
                }
                else
                {
                    // Display the specific error message from the Backend/Validator
                    MessageBox.Show(result.Message, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"System Error: {ex.Message}", "Error");
            }
        }
    }
}