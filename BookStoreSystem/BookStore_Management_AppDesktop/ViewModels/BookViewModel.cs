using BookStore_Management_AppDesktop.Models;
using BookStore_Management_AppDesktop.Services.API;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class BookViewModel : ObservableObject
    {
        private readonly BookApiService _apiService;

        [ObservableProperty]
        private ObservableCollection<Book> _books = new ObservableCollection<Book>();

        public BookViewModel()
        {
            _apiService = new BookApiService();
        }

        public async Task LoadDataAsync()
        {
            var booksFromApi = await _apiService.GetAllBooksAsync();

            Books = new ObservableCollection<Book>(booksFromApi);
        }
    }
}