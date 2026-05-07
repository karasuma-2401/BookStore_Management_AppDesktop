
using BookStore_Management_AppDesktop.Helpers.Enums;
using BookStore_Management_AppDesktop.Services.Navigation; 
using BookStore_Management_AppDesktop.ViewModels;
using System.Windows;
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
        public MainWindow(MainViewModel viewModel, INavigationService navigationService, Wpf.Ui.IContentDialogService contentDialogService)
        {
            InitializeComponent();

            this.DataContext = viewModel;

            navigationService.SetFrame(RootFrame);

            contentDialogService.SetContentPresenter(RootContentDialogPresenter);

            navigationService.NavigateTo(PageType.Books);
        }
    }
}