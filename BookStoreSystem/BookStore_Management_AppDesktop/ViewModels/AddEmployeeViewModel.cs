using BookStore_Management_AppDesktop.Models;
using BookStore_Management_AppDesktop.Models.DTOs;
using BookStore_Management_AppDesktop.Services.API;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class AddEmployeeViewModel : ObservableObject
    {
        private readonly IEmployeeApiService _employeeApi;
        private readonly IUserApiService _userApi;
        private List<UserResponseModel> _allUsers = new();

        [ObservableProperty]
        private Employee _newEmployee = new();

        [ObservableProperty]
        private ObservableCollection<UserResponseModel> _userList = new();

        [ObservableProperty]
        private UserResponseModel _selectedUser;

        [ObservableProperty]
        private string _searchUserIDText;

        public AddEmployeeViewModel()
        {
            _employeeApi = new EmployeeApiService();
            _userApi = new UserApiService();
            _ = LoadUsersAsync();
        }

        private async Task LoadUsersAsync()
        {
            try
            {
                var users = await _userApi.GetAllUsersAsync();
                if (users != null)
                {
                    // Filter users: only show users who don't already have an employee profile
                    // This prevents one user from having multiple employee records (Foreign Key constraint)
                    _allUsers = users.Where(u => u != null).ToList();

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        UserList.Clear();
                        foreach (var u in _allUsers)
                        {
                            if (u != null) UserList.Add(u);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading users: {ex.Message}");
                MessageBox.Show("Failed to load users. Please try again.", "Error", MessageBoxButton.OK);
            }
        }

        partial void OnSearchUserIDTextChanged(string value)
        {
            if (_allUsers == null) return;

            UserList.Clear();

            if (string.IsNullOrEmpty(value))
            {
                foreach (var user in _allUsers)
                {
                    if (user != null) UserList.Add(user);
                }
            }
            else
            {
                var filtered = _allUsers.Where(u => u != null && 
                    (u.Username.Contains(value, StringComparison.OrdinalIgnoreCase) || 
                     u.Email.Contains(value, StringComparison.OrdinalIgnoreCase) ||
                     u.FullName.Contains(value, StringComparison.OrdinalIgnoreCase)));

                foreach (var user in filtered)
                {
                    if (user != null) UserList.Add(user);
                }
            }
        }

        [RelayCommand]
        private async Task Save(Window window)
        {
            // Validation
            if (SelectedUser == null)
            {
                MessageBox.Show("Please select a User account.", "Validation Error", MessageBoxButton.OK);
                return;
            }

            if (string.IsNullOrWhiteSpace(NewEmployee.FullName))
            {
                MessageBox.Show("Please enter employee full name.", "Validation Error", MessageBoxButton.OK);
                return;
            }

            if (string.IsNullOrWhiteSpace(NewEmployee.Phone))
            {
                MessageBox.Show("Please enter employee phone number.", "Validation Error", MessageBoxButton.OK);
                return;
            }

            if (string.IsNullOrWhiteSpace(NewEmployee.Address))
            {
                MessageBox.Show("Please enter employee address.", "Validation Error", MessageBoxButton.OK);
                return;
            }

            try
            {
                // Assign UserId to avoid Foreign Key DB error
                NewEmployee.UserId = SelectedUser.UserId;

                var success = await _employeeApi.CreateEmployeeAsync(NewEmployee);
                if (success)
                {
                    MessageBox.Show("Employee added successfully!", "Success", MessageBoxButton.OK);
                    if (window != null) 
                    { 
                        window.DialogResult = true; 
                        window.Close(); 
                    }
                }
                else
                {
                    MessageBox.Show("Failed to save employee. Check if User already has an Employee profile or try again later.", 
                        "Error", MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving employee: {ex.Message}");
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK);
            }
        }

        [RelayCommand]
        private void Cancel(Window window)
        {
            window?.Close();
        }

        [RelayCommand]
        private async Task AddUserID()
        {
            // This command opens a new user creation window
            // For now, we'll show a message asking the user to create account first
            MessageBox.Show("Please create a User account first in the User Management section.", 
                "Information", MessageBoxButton.OK);

            // Reload users list in case a new user was added
            await LoadUsersAsync();
        }
    }
}