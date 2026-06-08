using BookStore_Management_AppDesktop.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using BookStore_Management_AppDesktop.Services;
using BookStore_Management_AppDesktop.Services.API;
using BookStore_Management_AppDesktop.Services.API.EmployeeServices;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class EmployeeViewModel : ObservableObject
    {
        private readonly IEmployeeApiService _apiService;
        private readonly IDialogService _dialogService;
        private readonly IUserApiService _userApiService;
        private List<Employee> _allEmployees = new();
        private CancellationTokenSource? _searchCts;
        private Dictionary<int, string> _userIdToRoleMap = new();

        private bool IsEmployeeAdmin(Employee emp)
        {
            if (emp == null) return false;
            if (_userIdToRoleMap.TryGetValue(emp.UserId, out var roleId))
            {
                return string.Equals(roleId, "admin", StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        [ObservableProperty]
        private ObservableCollection<Employee> _employees = new();

        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private string _selectedFilter = "Full Name";

        [ObservableProperty]
        private string _searchPlaceholder = "Search by Full Name...";

        public bool IsFullNameSelected => SelectedFilter == "Full Name";
        public bool IsSalarySelected => SelectedFilter == "Salary";
        public bool IsAddressSelected => SelectedFilter == "Address";

        [ObservableProperty]
        private int _currentPage = 1;

        [ObservableProperty]
        private int _pageSize = 8;

        public List<int> PageSizeOptions { get; set; } = new List<int> { 5, 8, 10, 12, 15 };

        public EmployeeViewModel(IEmployeeApiService apiService, IDialogService dialogService, IUserApiService userApiService)
        {
            _apiService = apiService;
            _dialogService = dialogService;
            _userApiService = userApiService;
            _ = InitializeDataAsync();
        }

        private async Task InitializeDataAsync()
        {
            try
            {
                var users = await _userApiService.GetAllUsersAsync();
                if (users != null)
                {
                    _userIdToRoleMap = users.ToDictionary(u => u.UserId, u => u.RoleId);
                }
                else
                {
                    _userIdToRoleMap.Clear();
                }

                var data = await _apiService.GetAllEmployeesAsync();
                if (data != null)
                {
                    _allEmployees = data;
                    UpdateDisplayList();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ViewModel Error: {ex.Message}");
            }
        }

        partial void OnSearchTextChanged(string value)
        {
            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();
            var token = _searchCts.Token;

            Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(400, token);
                    if (!token.IsCancellationRequested)
                    {
                        CurrentPage = 1;
                        UpdateDisplayList();
                    }
                }
                catch (OperationCanceledException) { }
            }, token);
        }

        partial void OnPageSizeChanged(int value)
        {
            CurrentPage = 1;
            UpdateDisplayList();
        }

        partial void OnCurrentPageChanged(int value) => UpdateDisplayList();

        private void UpdateDisplayList()
        {
            var filteredData = _allEmployees.Where(e =>
            {
                if (string.IsNullOrWhiteSpace(SearchText)) return true;
                string search = SearchText.ToLower();
                return SelectedFilter switch
                {
                    "Full Name" => e.FullName?.ToLower().Contains(search) ?? false,
                    "Address" => e.Address?.ToLower().Contains(search) ?? false,
                    "Salary" => e.Salary.ToString().Contains(search),
                    _ => true
                };
            }).ToList();

            // Sort: Admin first, then by Full Name
            filteredData = filteredData
                .OrderByDescending(e => IsEmployeeAdmin(e))
                .ThenBy(e => e.FullName)
                .ToList();

            int totalFiltered = filteredData.Count;
            int totalPages = Math.Max(1, (int)Math.Ceiling((double)totalFiltered / PageSize));

            Application.Current.Dispatcher.Invoke(() =>
            {
                if (CurrentPage > totalPages) CurrentPage = totalPages;

                var itemsToShow = filteredData
                    .Skip((CurrentPage - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();

                Employees.Clear();
                foreach (var emp in itemsToShow) Employees.Add(emp);

                OnPropertyChanged(nameof(TotalEmployees));
                OnPropertyChanged(nameof(TotalPages));
                OnPropertyChanged(nameof(CurrentPageStart));
                OnPropertyChanged(nameof(CurrentPageEnd));
                OnPropertyChanged(nameof(TotalActiveEmployees));
                OnPropertyChanged(nameof(TotalResignedEmployees));
                OnPropertyChanged(nameof(TotalAllEmployees));
            });
        }

        public int TotalActiveEmployees => _allEmployees.Count(e => e.Status == 1);
        public int TotalResignedEmployees => _allEmployees.Count(e => e.Status == 0);
        public int TotalAllEmployees => _allEmployees.Count;

        public int TotalEmployees => _allEmployees.Count(e => {
            if (string.IsNullOrWhiteSpace(SearchText)) return true;
            string search = SearchText.ToLower();
            return SelectedFilter switch
            {
                "Full Name" => e.FullName?.ToLower().Contains(search) ?? false,
                "Address" => e.Address?.ToLower().Contains(search) ?? false,
                "Salary" => e.Salary.ToString().Contains(search),
                _ => true
            };
        });

        public int TotalPages => Math.Max(1, (int)Math.Ceiling((double)TotalEmployees / PageSize));
        public int CurrentPageStart => TotalEmployees == 0 ? 0 : (CurrentPage - 1) * PageSize + 1;
        public int CurrentPageEnd => Math.Min(CurrentPage * PageSize, TotalEmployees);

        // --- ACTION COMMANDS ---

        [RelayCommand]
        private async Task EditEmployee(Employee employee)
        {
            if (employee == null) return;

            var editViewModel = new UpdateEmployeeViewModel(employee, IsEmployeeAdmin(employee));
            var editWindow = new BookStore_Management_AppDesktop.Views.Windows.EditEmployeeWindow(editViewModel);

            if (Application.Current.MainWindow != null)
            {
                editWindow.Owner = Application.Current.MainWindow;
            }

            if (editWindow.ShowDialog() == true)
            {
                await InitializeDataAsync();
            }
        }

        [RelayCommand]
        private async Task DeleteEmployee(Employee employee)
        {
            if (employee == null) return;

            bool isAdmin = IsEmployeeAdmin(employee);
            string noun = isAdmin ? "Admin" : "Employee";

            bool isConfirmed = _dialogService.ShowConfirmation(
                message: $"Are you sure you want to change status of {noun.ToLower()} '{employee.FullName}' to Resigned (Nghỉ làm)?",
                confirmText: "Change",
                isDanger: true);

            if (isConfirmed)
            {
                try
                {
                    bool isEmployeeDeleted = await _apiService.DeleteEmployeeAsync(employee.EmployeeId);

                    if (isEmployeeDeleted)
                    {
                        // Soft delete: update status to 0 locally
                        employee.Status = 0;
                        UpdateDisplayList();
                        _dialogService.ShowMessage($"{noun} status updated to Resigned (Nghỉ làm) successfully!");
                    }
                    else
                    {
                        _dialogService.ShowMessage($"Failed to update {noun.ToLower()} status.");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[DeleteEmployee Error]: {ex}");
                    _dialogService.ShowMessage($"System Error: {ex.Message}");
                }
            }
        }

        private async Task<bool> DeleteUserAccountAsync(int userId)
        {
            try
            {
                using (var client = new HttpClient { BaseAddress = new Uri("https://localhost:7063/") })
                {
                    var token = Settings.Default.AccessToken;
                    if (!string.IsNullOrEmpty(token))
                    {
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    }

                    var response = await client.DeleteAsync($"api/users/{userId}");
                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"User Delete Error: {ex.Message}");
                return false;
            }
        }

        [RelayCommand]
        private void ChangeFilter(string filterType)
        {
            SelectedFilter = filterType;
            SearchPlaceholder = $"Search by {filterType}...";
            OnPropertyChanged(nameof(IsFullNameSelected));
            OnPropertyChanged(nameof(IsSalarySelected));
            OnPropertyChanged(nameof(IsAddressSelected));
            UpdateDisplayList();
        }

        [RelayCommand]
        private void NextPage() { if (CurrentPage < TotalPages) CurrentPage++; }

        [RelayCommand]
        private void PreviousPage() { if (CurrentPage > 1) CurrentPage--; }

        [RelayCommand]
        private async Task OpenAddEmployeeWindow()
        {
            var addWin = new BookStore_Management_AppDesktop.Views.Windows.AddEmployeeWindow();
            if (Application.Current.MainWindow != null) addWin.Owner = Application.Current.MainWindow;
            if (addWin.ShowDialog() == true) await InitializeDataAsync();
        }

        [RelayCommand]
        private async Task ChangePassword(Employee employee)
        {
            if (employee == null) return;

            bool isAdmin = IsEmployeeAdmin(employee);
            string noun = isAdmin ? "Admin" : "Employee";

            var dialog = new BookStore_Management_AppDesktop.Views.Windows.ChangeEmployeePasswordDialog(employee.FullName);
            if (Application.Current.MainWindow != null)
            {
                dialog.Owner = Application.Current.MainWindow;
            }

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var result = await _userApiService.AdminChangeStaffPasswordAsync(employee.EmployeeId, dialog.NewPassword);
                    if (result.IsSuccess)
                    {
                        _dialogService.ShowMessage($"{noun} password updated successfully!");
                    }
                    else
                    {
                        _dialogService.ShowMessage($"Failed to update {noun.ToLower()} password: {result.Message}");
                    }
                }
                catch (Exception ex)
                {
                    _dialogService.ShowMessage($"Error: {ex.Message}");
                }
            }
        }

        [RelayCommand]
        private void CloseWindow(Window window) => window?.Close();
    }
}