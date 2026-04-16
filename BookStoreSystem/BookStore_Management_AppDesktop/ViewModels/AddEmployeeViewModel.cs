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

        // Original list for searching/filtering if needed
        private List<int> _allUnassignedUserIds = new();

        [ObservableProperty]
        private Employee _newEmployee = new();

        [ObservableProperty]
        private ObservableCollection<int> _userList = new();

        // Stores the ID selected from the ComboBox
        [ObservableProperty]
        private int _selectedUserId;

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
                // 1. Call User and Employee APIs in parallel to optimize time
                var usersTask = _userApi.GetAllUsersAsync();
                var employeesTask = _employeeApi.GetAllEmployeesAsync();

                await Task.WhenAll(usersTask, employeesTask);

                var allUsers = await usersTask;
                var allEmployees = await employeesTask;

                if (allUsers != null && allEmployees != null)
                {
                    // 2. Find UserIds already assigned to employees (Foreign Key check)
                    var assignedUserIds = allEmployees
                                          .Select(e => e.UserId)
                                          .Distinct()
                                          .ToHashSet();

                    // 3. Filter IDs that are not yet used
                    _allUnassignedUserIds = allUsers
                        .Where(u => u != null && !assignedUserIds.Contains(u.UserId))
                        .Select(u => u.UserId)
                        .ToList();

                    // 4. Update the UI on the Dispatcher thread
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        UserList.Clear();
                        foreach (var id in _allUnassignedUserIds)
                        {
                            UserList.Add(id);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                MessageBox.Show("Unable to load User ID list.", "System Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task AddEmployee(Window window)
        {
            // Validation: Check if an ID has been selected
            if (SelectedUserId <= 0)
            {
                MessageBox.Show("Please select a valid User ID.", "Input Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(NewEmployee.FullName))
            {
                MessageBox.Show("Please enter the employee's full name.", "Input Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Assign selected ID to the new employee object
                NewEmployee.UserId = SelectedUserId;

                var success = await _employeeApi.CreateEmployeeAsync(NewEmployee);
                if (success)
                {
                    MessageBox.Show("Employee added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    if (window != null)
                    {
                        window.DialogResult = true;
                        window.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Save failed. Please check the data or ensure the account doesn't already have a profile.", "Server Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"System Error: {ex.Message}", "Critical Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void Cancel(Window window) => window?.Close();

        [RelayCommand]
        private async Task AddUserID()
        {
            var addUserWin = new BookStore_Management_AppDesktop.Views.Windows.AddUserWindow();
            var addUserVM = new AddUserViewModel();
            addUserWin.DataContext = addUserVM;

            if (addUserWin.ShowDialog() == true)
            {
                await LoadUsersAsync();

                if (UserList.Any())
                {
                    SelectedUserId = UserList.Max();
                }
            }
        }
    }
}