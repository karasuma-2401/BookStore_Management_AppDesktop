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
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Threading.Tasks;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class EmployeeViewModel : ObservableObject
    {
        //Danh sách gốc chứa tất cả nhân viên
        private List<Employee> _allEmployees = new();
        // Danh sách nhân viên hiển thị trên datagrid
        [ObservableProperty]
        private ObservableCollection<Employee> _employees = new();

        // 1. CẤU HÌNH PHÂN TRANG
        // Trang hiện tại đang hiển thị (Mặc định là 1)
        [ObservableProperty]
        private int _currentPage = 1;

        // Số lượng nhân viên hiển thị trên một trang (Mặc định là 10)
        [ObservableProperty]
        private int _pageSize = 8;

        public List<int> PageSizeOptions { get; set; } = new List<int> { 5, 8, 10, 12, 15 };

        //Các thuộc tính hỗ trợ hiển thị UI (Dòng bắt đầu , dòng kết thúc, tổng số trang)
        public int TotalEmployees => _allEmployees.Count;
        public int TotalPages => (int)Math.Ceiling((double)TotalEmployees / PageSize);
        public int CurrentPageStart => (CurrentPage - 1) * PageSize + 1;
        public int CurrentPageEnd => Math.Min(CurrentPage * PageSize, TotalEmployees);
        // 2. QUẢN LÝ AVATAR
        // Ảnh thực tế đang hiển thị của người dùng
        [ObservableProperty]
        private string _currentAvatarPath = "pack://application:,,,/Resources/Images/default_user.png";

        // Biến tạm để chứa ảnh đang chọn trong cửa sổ Modal (chưa lưu)
        [ObservableProperty]
        private string _tempAvatarPath;

        // Danh sách ảnh mặc định (Đảm bảo Build Action của các file này là "Resource")
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

        private readonly IEmployeeApiService _apiService; // Thêm service


        public EmployeeViewModel()
        {
            _apiService = new EmployeeApiService(); // Khởi tạo thực tế
            _ = InitializeDataAsync(); // Kéo dữ liệu khi vừa mở trang
        }

        private async Task InitializeDataAsync()
        {
            try
            {
                // Hiển thị trạng thái đang tải (nếu có biến IsBusy)
                var data = await _apiService.GetAllEmployeesAsync();

                if (data != null && data.Count > 0)
                {
                    _allEmployees = data;
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        UpdateDisplayList();
                    });
                }
                else
                {
                    // Nếu data rỗng, có thể do Token hết hạn hoặc không phải Admin
                    System.Diagnostics.Debug.WriteLine("Không nhận được dữ liệu hoặc quyền truy cập bị từ chối.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi ViewModel: {ex.Message}");
            }
        }

        // Tự động chạy khi PageSize thay đổi (tính năng của MVVM Toolkit)
        partial void OnPageSizeChanged(int value)
        {
            CurrentPage = 1; // Reset về trang 1 khi đổi số dòng hiển thị
            UpdateDisplayList();
        }

        // Tự động chạy khi CurrentPage thay đổi
        partial void OnCurrentPageChanged(int value)
        {
            UpdateDisplayList();
        }
        void UpdateDisplayList()
        {
            Employees.Clear();
            var itemsToShow = _allEmployees.Skip((CurrentPage - 1) * PageSize).Take(PageSize);
            foreach (var emp in itemsToShow)
            {
                Employees.Add(emp);
            }
            OnPropertyChanged(nameof(TotalEmployees));
            OnPropertyChanged(nameof(TotalPages));
            OnPropertyChanged(nameof(CurrentPageStart));
            OnPropertyChanged(nameof(CurrentPageEnd));
        }

        [RelayCommand]
        private void NextPage()
        {
            if (CurrentPage < TotalPages) CurrentPage++;
        }

        [RelayCommand]
        private void PreviousPage()
        {
            if (CurrentPage > 1) CurrentPage--;
        }

        [RelayCommand]
        private void OpenAvatarModal()
        {
            // Reset ảnh tạm về ảnh hiện tại trước khi mở modal
            TempAvatarPath = CurrentAvatarPath;

            var editWindow = new BookStore_Management_AppDesktop.Views.Windows.AvatarEditView(this);

            if (Application.Current.MainWindow != null)
            {
                editWindow.Owner = Application.Current.MainWindow;
            }

            editWindow.ShowDialog();
        }


        [RelayCommand]
        private void UploadFromComputer()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpeg;*.jpg;*.webp)|*.png;*.jpeg;*.jpg;*.webp",
                Title = "Chọn ảnh đại diện"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                TempAvatarPath = openFileDialog.FileName;
            }
        }


        [RelayCommand]
        private void SelectDefaultAvatar(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                TempAvatarPath = path;
            }
        }

        // Đóng cửa số avatar
        [RelayCommand]
        private void CloseWindow(Window window)
        {
            if (window != null)
            {
                window.Close();
            }
        }

        [RelayCommand]
        private void SaveAvatarChange(Window window)
        {
            // Cập nhật ảnh chính thức từ ảnh tạm
            CurrentAvatarPath = TempAvatarPath;

            // Đóng cửa sổ
            window?.Close();
        }
    }
}