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

        // Danh sách gốc để phục vụ việc search/filter nếu cần
        private List<int> _allUnassignedUserIds = new();

        [ObservableProperty]
        private Employee _newEmployee = new();

        // Sửa kiểu dữ liệu thành int để tránh lỗi CS1503
        [ObservableProperty]
        private ObservableCollection<int> _userList = new();

        // Lưu trữ ID được chọn từ ComboBox
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
                // 1. Gọi song song API User và Employee để tối ưu thời gian
                var usersTask = _userApi.GetAllUsersAsync();
                var employeesTask = _employeeApi.GetAllEmployeesAsync();

                await Task.WhenAll(usersTask, employeesTask);

                var allUsers = await usersTask;
                var allEmployees = await employeesTask;

                if (allUsers != null && allEmployees != null)
                {
                    // 2. Tìm các UserId đã được cấp cho nhân viên (khóa ngoại)
                    var assignedUserIds = allEmployees
                                          .Select(e => e.UserId)
                                          .Distinct()
                                          .ToHashSet();

                    // 3. Lọc ra danh sách ID chưa được sử dụng
                    _allUnassignedUserIds = allUsers
                        .Where(u => u != null && !assignedUserIds.Contains(u.UserId))
                        .Select(u => u.UserId)
                        .ToList();

                    // 4. Cập nhật giao diện trên UI Thread
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        UserList.Clear();
                        foreach (var id in _allUnassignedUserIds)
                        {
                            UserList.Add(id); // Thêm kiểu int vào ObservableCollection<int>
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                MessageBox.Show("Không thể tải danh sách ID người dùng.");
            }
        }

        [RelayCommand]
        private async Task Save(Window window)
        {
            // Validation: Kiểm tra ID đã được chọn chưa
            if (SelectedUserId <= 0)
            {
                MessageBox.Show("Vui lòng chọn một User ID hợp lệ.", "Lỗi nhập liệu", MessageBoxButton.OK);
                return;
            }

            if (string.IsNullOrWhiteSpace(NewEmployee.FullName))
            {
                MessageBox.Show("Vui lòng nhập tên nhân viên.", "Lỗi nhập liệu", MessageBoxButton.OK);
                return;
            }

            try
            {
                // Gán ID trực tiếp từ ComboBox vào đối tượng nhân viên mới
                NewEmployee.UserId = SelectedUserId;

                var success = await _employeeApi.CreateEmployeeAsync(NewEmployee);
                if (success)
                {
                    MessageBox.Show("Thêm nhân viên thành công!", "Thông báo", MessageBoxButton.OK);
                    if (window != null)
                    {
                        window.DialogResult = true;
                        window.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Lưu thất bại. Kiểm tra lại dữ liệu hoặc tài khoản đã có hồ sơ.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi hệ thống: {ex.Message}");
            }
        }

        [RelayCommand]
        private void Cancel(Window window) => window?.Close();
    }
}