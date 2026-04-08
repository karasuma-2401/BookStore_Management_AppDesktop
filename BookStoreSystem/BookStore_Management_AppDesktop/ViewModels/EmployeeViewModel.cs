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

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class EmployeeViewModel : ObservableObject
    {
        private readonly IEmployeeApiService _apiService;
        private List<Employee> _allEmployees = new();

        // Token để hủy việc search cũ khi người dùng đang gõ phím liên tục (Debounce)
        private CancellationTokenSource? _searchCts;

        [ObservableProperty]
        private ObservableCollection<Employee> _employees = new();

        // --- SEARCH & FILTER PROPERTIES ---
        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private string _selectedFilter = "Full Name";

        [ObservableProperty]
        private string _searchPlaceholder = "Search by Full Name...";

        // Properties giúp ContextMenu hiển thị dấu tích hoặc thanh màu xanh nổi bật
        public bool IsFullNameSelected => SelectedFilter == "Full Name";
        public bool IsSalarySelected => SelectedFilter == "Salary";
        public bool IsAddressSelected => SelectedFilter == "Address";

        // --- PAGINATION PROPERTIES ---
        [ObservableProperty]
        private int _currentPage = 1;

        [ObservableProperty]
        private int _pageSize = 8;

        public List<int> PageSizeOptions { get; set; } = new List<int> { 5, 8, 10, 12, 15 };

        // --- AVATAR PROPERTIES ---
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

        // --- LOGIC DEBOUNCE SEARCH (Khắc phục giật lag) ---
        partial void OnSearchTextChanged(string value)
        {
            // Hủy đợt chờ search cũ
            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();
            var token = _searchCts.Token;

            Task.Run(async () =>
            {
                try
                {
                    // Chờ người dùng ngừng gõ trong 400ms mới bắt đầu lọc
                    await Task.Delay(400, token);

                    if (!token.IsCancellationRequested)
                    {
                        // Reset về trang 1 khi thực hiện tìm kiếm mới
                        CurrentPage = 1;
                        UpdateDisplayList();
                    }
                }
                catch (OperationCanceledException) { /* Bỏ qua nếu bị hủy bởi phím gõ tiếp theo */ }
            }, token);
        }

        partial void OnPageSizeChanged(int value)
        {
            CurrentPage = 1;
            UpdateDisplayList();
        }

        partial void OnCurrentPageChanged(int value) => UpdateDisplayList();

        // --- CORE FILTER & PAGINATION LOGIC ---
        private void UpdateDisplayList()
        {
            // 1. Lọc dữ liệu trên Thread phụ dựa trên SearchText và Option đã chọn
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

            // 2. Tính toán các thông số phân trang
            int totalFiltered = filteredData.Count;
            int totalPages = Math.Max(1, (int)Math.Ceiling((double)totalFiltered / PageSize));

            // Đảm bảo các thay đổi UI diễn ra trên UI Thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (CurrentPage > totalPages) CurrentPage = totalPages;

                var itemsToShow = filteredData
                    .Skip((CurrentPage - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();

                Employees.Clear();
                foreach (var emp in itemsToShow) Employees.Add(emp);

                // Thông báo cập nhật các thuộc tính tính toán (Getters) và UI
                OnPropertyChanged(nameof(TotalEmployees));
                OnPropertyChanged(nameof(TotalPages));
                OnPropertyChanged(nameof(CurrentPageStart));
                OnPropertyChanged(nameof(CurrentPageEnd));
            });
        }

        // --- CALCULATED GETTERS CHO UI ---
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

        // --- COMMANDS ---

        [RelayCommand]
        private void ChangeFilter(string filterType)
        {
            SelectedFilter = filterType;
            SearchPlaceholder = $"Search by {filterType}...";

            // Cập nhật trạng thái Highlight cho ContextMenu
            OnPropertyChanged(nameof(IsFullNameSelected));
            OnPropertyChanged(nameof(IsSalarySelected));
            OnPropertyChanged(nameof(IsAddressSelected));

            // Lọc lại dữ liệu ngay khi đổi chế độ
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

        // Avatar Actions
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