using BookStore_Management_AppDesktop.Helpers.Enums;
using BookStore_Management_AppDesktop.Services.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BookStore_Management_AppDesktop.ViewModels
{
    // Kế thừa ObservableObject để tự động cập nhật UI khi dữ liệu thay đổi
    public partial class MainViewModel : ObservableObject
    {
        // Khai báo Service điều hướng
        private readonly INavigationService _navigationService;

        

        // Dependency Injection:
        public MainViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

        }

        /// <summary>
        /// Hàm này sẽ tự động chạy khi người dùng click vào bất kỳ nút nào trên Menu
        /// </summary>
        /// <param name="pageName">Chính là Tag của nút (ví dụ: "Books", "Employees")</param>
        [RelayCommand]
        private void Navigate(string pageName)
        {
            // 1. Chuyển đổi chuỗi String thành Enum một cách an toàn
            if (Enum.TryParse(pageName, out PageType pageType))
            {
                // 2. Ra lệnh cho Service nạp trang đó lên Frame
                _navigationService.NavigateTo(pageType);
            }
        }
    }
}
