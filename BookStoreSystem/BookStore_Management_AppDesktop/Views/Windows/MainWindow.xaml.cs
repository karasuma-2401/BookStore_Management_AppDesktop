
using BookStore_Management_AppDesktop.Helpers.Enums;
using BookStore_Management_AppDesktop.ViewModels;
using System.Windows;
using BookStore_Management_AppDesktop.Services.Navigation; 
namespace BookStore_Management_AppDesktop.Views.Windows
{
    /// <summary>
    /// Cửa sổ chính của ứng dụng (Main Layout).
    /// Đóng vai trò là lớp vỏ (Shell) chứa Sidebar Menu và vùng hiển thị nội dung động (Content Region).
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Cửa sổ chính của ứng dụng (Main Layout).
        /// Đóng vai trò là lớp vỏ (Shell) chứa Sidebar Menu và vùng hiển thị nội dung động (Content Region).
        /// </summary>
        public MainWindow(MainViewModel viewModel, INavigationService navigationService)
        {
            InitializeComponent();

            // Thiết lập ngữ cảnh dữ liệu (DataContext) để kích hoạt cơ chế Data Binding của MVVM
            this.DataContext = viewModel;

            navigationService.SetFrame(RootFrame);

            navigationService.NavigateTo(PageType.Books);
        }
    }
}