using BookStore_Management_AppDesktop.Services;
using BookStore_Management_AppDesktop.Services.API;
using BookStore_Management_AppDesktop.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace BookStore_Management_AppDesktop.Views.Windows
{
    /// <summary>
    /// Interaction logic for AddBookWindow.xaml
    /// </summary>
    public partial class AddBookWindow : Window
    {
        private readonly BookFormViewModel _viewModel;

        public AddBookWindow()
        {
            InitializeComponent();

            // 1. Khởi tạo các Services
            var authorApiService = new AuthorApiService();
            var bookApiService = new BookApiService();
            var cloudinaryService = new CloudinaryService();

            // 2. Khởi tạo AuthorSelectionViewModel (Khối Lego Tác giả)
            var authorVM = new AuthorSelectionViewModel(authorApiService);

            // 3. Tiêm tất cả vào BookFormViewModel (Không truyền Book -> Tự hiểu là chế độ Add)
            _viewModel = new BookFormViewModel(bookApiService, cloudinaryService, authorVM);

            _viewModel.OnShowMessage = (message) =>
            {
                var msgBox = new CustomMessageBox(message);
                msgBox.ShowDialog();
            };

            _viewModel.OnRequestClose = () => this.Close();

            this.DataContext = _viewModel;

            // Gắn sự kiện để load dữ liệu khi mở form
            this.Loaded += AddBookWindow_Loaded;
        }

        private async void AddBookWindow_Loaded(object sender, RoutedEventArgs e)
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