using BookStore_Management_AppDesktop.Models;
using BookStore_Management_AppDesktop.Services;
using BookStore_Management_AppDesktop.Services.API;
using BookStore_Management_AppDesktop.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace BookStore_Management_AppDesktop.Views.Windows
{
    /// <summary>
    /// Interaction logic for EditBookWindow.xaml
    /// </summary>
    public partial class EditBookWindow : Window
    {
        private readonly BookFormViewModel _viewModel;

        // Yêu cầu phải truyền vào một đối tượng Book khi mở cửa sổ này
        public EditBookWindow(Book bookToEdit)
        {
            InitializeComponent();

            // 1. Khởi tạo các Services
            var authorApiService = new AuthorApiService();
            var bookApiService = new BookApiService();
            var cloudinaryService = new CloudinaryService();

            // 2. Khởi tạo AuthorSelectionViewModel (Khối Lego Tác giả)
            var authorVM = new AuthorSelectionViewModel(authorApiService);

            // 3. Tiêm tất cả vào BookFormViewModel (CÓ truyền bookToEdit -> Tự hiểu là chế độ Edit)
            _viewModel = new BookFormViewModel(bookApiService, cloudinaryService, authorVM, bookToEdit);

            _viewModel.OnShowMessage = (message) =>
            {
                var msgBox = new CustomMessageBox(message);
                msgBox.ShowDialog();
            };

            _viewModel.OnRequestClose = () => this.Close();

            this.DataContext = _viewModel;

            // ĐÃ BỔ SUNG: Gắn sự kiện để load dữ liệu (và tự động chọn tác giả cũ) khi mở form
            this.Loaded += EditBookWindow_Loaded;
        }

        private async void EditBookWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.InitializeAsync();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
    }
}