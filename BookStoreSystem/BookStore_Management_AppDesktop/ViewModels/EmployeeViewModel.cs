using BookStore_Management_AppDesktop.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class EmployeeViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<Employee> _employees;
        // Danh sách các lựa chọn cho số dòng trên mỗi trang
        public List<int> PageSizeOptions { get; set; } = new List<int> { 4, 8, 12, 16 };

        private int _pageSize = 8;
        public int PageSize
        {
            get => _pageSize;
            set => SetProperty(ref _pageSize, value); // Thông báo cho UI khi giá trị thay đổi
        }
        public EmployeeViewModel()
        {
            Employees = new ObservableCollection<Employee>();

            LoadFakeData();
        }

        private void LoadFakeData()
        {
            for (int i = 0; i < 30; i++)
            {
                _employees.Add(new Employee
                {
                    UserId = "AABBCC123",
                    EmployeeId = "DDEEFF456",
                    Username = "abc123",
                    Password = "123456",
                    Address = "123 Đường ABC",
                    PhoneNumber = "0123456789",
                    FullName = "Trần Văn A" + i
                });
            }
        }
    }
}