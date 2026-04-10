using BookStore_Management_AppDesktop.Models;
using BookStore_Management_AppDesktop.Services.API;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;


namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class EmployeeViewModel : ObservableObject
    {
        private readonly IEmployeeApiService _apiService;
        private List<Employee> _allEmployees = new();
        private CancellationTokenSource? _searchCts;

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

        [ObservableProperty]
        private string _currentAvatarPath = "pack://application:,,,/Resources/Images/default_user.png";

        [ObservableProperty]
        private string _tempAvatarPath;

        public ObservableCollection<string> DefaultAvatars { get; set; } = new()
        {
            "pack://siteoforigin:,,,/Resources/Images/Forged of Fire.webp",
            "pack://siteoforigin:,,,/Resources/Images/In an Instant.webp",
            "pack://siteoforigin:,,,/Resources/Images/In Five Year.webp",
            "pack://siteoforigin:,,,/Resources/Images/Night Road.webp",
            "pack://siteoforigin:,,,/Resources/Images/Promise Me.webp",
            "pack://siteoforigin:,,,/Resources/Images/Magic Hour A novel.webp",
            "pack://siteoforigin:,,,/Resources/Images/God of Malice A Dark College.webp",
            "pack://siteoforigin:,,,/Resources/Images/Twenty Years Later.webp"
        };

        public EmployeeViewModel()
        {
            _apiService = new EmployeeApiService();
            _ = InitializeDataAsync();
        }

        private async Task InitializeDataAsync()
        {
            try
            {
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
            });
        }

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


            var editViewModel = new UpdateEmployeeViewModel(employee);

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

            var confirmDialog = new BookStore_Management_AppDesktop.Views.Windows.DeleteConfirmationWindow();
            if (Application.Current.MainWindow != null)
            {
                confirmDialog.Owner = Application.Current.MainWindow;
            }

            if (confirmDialog.ShowDialog() == true)
            {
                try
                {
                    bool isEmployeeDeleted = await _apiService.DeleteEmployeeAsync(employee.EmployeeId);

                    if (isEmployeeDeleted)
                    {
                        bool isUserDeleted = await DeleteUserAccountAsync(employee.UserId);

                        _allEmployees.Remove(employee);
                        UpdateDisplayList();

                        if (isUserDeleted)
                        {
                            MessageBox.Show("Employee and associated User account deleted successfully!",
                                            "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Employee deleted, but the User account could not be removed. It might be in use elsewhere.",
                                            "Partial Success", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete the Employee record.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"System Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
        private void OpenAvatarModal()
        {
            TempAvatarPath = CurrentAvatarPath;
            var editWindow = new BookStore_Management_AppDesktop.Views.Windows.AvatarEditView(this);
            if (Application.Current.MainWindow != null) editWindow.Owner = Application.Current.MainWindow;
            editWindow.ShowDialog();
        }

        [RelayCommand]
        private void SaveAvatarChange(Window window)
        {
            CurrentAvatarPath = TempAvatarPath;
            window?.Close();
        }

        [RelayCommand]
        private void SelectDefaultAvatar(string path) => TempAvatarPath = path;

        [RelayCommand]
        private void UploadFromComputer()
        {
            var openFileDialog = new OpenFileDialog { Filter = "Images|*.png;*.jpg;*.jpeg;*.webp" };
            if (openFileDialog.ShowDialog() == true) TempAvatarPath = openFileDialog.FileName;
        }

        [RelayCommand]
        private void CloseWindow(Window window) => window?.Close();
    }
}