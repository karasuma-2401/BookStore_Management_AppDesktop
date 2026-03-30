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
        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
            }
        }
        public InventoryViewModel()
        {
            _apiService = new BookApiService();

            _ = LoadDataAsync();
        }

        public async Task LoadDataAsync()
        {
            var booksFromApi = await _apiService.GetAllBooksAsync();

            Books = new ObservableCollection<Book>(booksFromApi);
        }

        public Action<string>? OnShowMessage { get; set; }

        public Func<string, string, Task<bool>>? OnRequestConfirm { get; set; }

        [RelayCommand]
        private async Task DeleteBookAsync(Book selectedBook)
        {
            if (selectedBook == null) return;

            bool isConfirmed = false;
            if (OnRequestConfirm != null)
            {
                isConfirmed = await OnRequestConfirm.Invoke("Confirm Deletion", $"Are you sure you want to delete the book '{selectedBook.Title}'?");
            }

            if (isConfirmed)
            {
                bool isSuccess = await _apiService.DeleteBookAsync(selectedBook.BookId);

                if (isSuccess)
                {
                    var cloudinaryService = new CloudinaryService();
                    await cloudinaryService.DeleteImageAsync(selectedBook.ImagePath); 

                    OnShowMessage?.Invoke("Book deleted successfully!");
                    await LoadDataAsync();
                }
                else
                {
                    OnShowMessage?.Invoke("Book deleted successfully!");
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
