using BookStore_Management_AppDesktop.Helpers;
using BookStore_Management_AppDesktop.Models;
using BookStore_Management_AppDesktop.Services;
using BookStore_Management_AppDesktop.Services.API;
using BookStore_Management_AppDesktop.Views.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class InventoryViewModel : ObservableObject
    {
        private readonly BookApiService _apiService;

        private ObservableCollection<Book> _books = new ObservableCollection<Book>();
        public ObservableCollection<Book> Books
        {
            get => _books;
            set => SetProperty(ref _books, value);
        }

        public InventoryViewModel()
        {
            _apiService = new BookApiService();

            _ = LoadDataAsync();
        }

        public async Task LoadDataAsync()
        {
            var booksFromApi = await _apiService.GetAllBooksAsync();

            Books.Clear();
            foreach (var book in booksFromApi)
            {
                Books.Add(book);
            }
        }

        [RelayCommand]
        private async Task DeleteBookAsync(Book selectedBook)
        {
            if (selectedBook == null) return;

            var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa cuốn sách '{selectedBook.Title}' không?",
                                         "Xác nhận xóa",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                bool isSuccess = await _apiService.DeleteBookAsync(selectedBook.BookId);

                if (isSuccess)
                {

                    var cloudinaryService = new CloudinaryService();
                    await cloudinaryService.DeleteImageAsync(selectedBook.ImagePath);

                    MessageBox.Show("Xóa sách thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    await LoadDataAsync();
                }
                else
                {
                    MessageBox.Show("Xóa thất bại! Vui lòng kiểm tra lại kết nối.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        private async Task EditBook(Book selectedBook)
        {
            if (selectedBook == null) return;

            var editWindow = new EditBookWindow(selectedBook);
            editWindow.ShowDialog(); 

            await LoadDataAsync();
        }
    }
}
