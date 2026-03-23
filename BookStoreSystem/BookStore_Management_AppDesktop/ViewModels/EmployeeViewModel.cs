using BookStore_Management_AppDesktop.Models;
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

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class EmployeeViewModel : ObservableObject
    {
        // 1. QUẢN LÝ DANH SÁCH NHÂN VIÊN
        [ObservableProperty]
        private ObservableCollection<Employee> _employees = new();

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

        // 3. CẤU HÌNH PHÂN TRANG
        public List<int> PageSizeOptions { get; set; } = new List<int> { 4, 8, 12, 16 };

        [ObservableProperty]
        private int _pageSize = 8;



        // CONSTRUCTOR
        public EmployeeViewModel()
        {
            LoadFakeData();
        }

        // --- COMMANDS ---

        /// <summary>
        /// Mở cửa sổ chỉnh sửa Avatar
        /// </summary>
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

        /// <summary>
        /// Chọn ảnh từ máy tính (Lưu vào biến tạm)
        /// </summary>
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

        /// <summary>
        /// Chọn ảnh từ danh sách mặc định (Lưu vào biến tạm)
        /// </summary>
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
        /// <summary>
        /// Xác nhận lưu thay đổi ảnh
        /// </summary>
        [RelayCommand]
        private void SaveAvatarChange(Window window)
        {
            // Cập nhật ảnh chính thức từ ảnh tạm
            CurrentAvatarPath = TempAvatarPath;

            // Đóng cửa sổ
            window?.Close();
        }

        // DỮ LIỆU GIẢ ĐỂ TEST
        private void LoadFakeData()
        {
            for (int i = 1; i <= 20; i++)
            {
                Employees.Add(new Employee
                {
                    UserId = i.ToString(),
                    EmployeeId = "NV" + i.ToString("D3"),
                    Username = "user_" + i,
                    FullName = "Nhân viên " + i,
                    Address = "Việt Nam",
                    PhoneNumber = "090123456" + (i % 10)
                });
            }
        }
    }
}