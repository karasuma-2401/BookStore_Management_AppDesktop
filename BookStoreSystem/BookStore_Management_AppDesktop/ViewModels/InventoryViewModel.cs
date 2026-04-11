using BookStore_Management_AppDesktop.Helpers;
using BookStore_Management_AppDesktop.Models;
using BookStore_Management_AppDesktop.Services;
using BookStore_Management_AppDesktop.Services.API;
using BookStore_Management_AppDesktop.Views.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using static System.Reflection.Metadata.BlobBuilder;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class InventoryViewModel : ObservableObject
    {
        private readonly IBookApiService _apiService;
        private readonly CloudinaryService _cloudinaryService;

        [ObservableProperty]
        private ObservableCollection<Book> _books = new ObservableCollection<Book>();

        [ObservableProperty]
        private string _searchText = string.Empty;

        public InventoryViewModel(IBookApiService apiService, CloudinaryService cloudinaryService)
        {
            _apiService = apiService;
            _cloudinaryService = cloudinaryService;

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
                    if (!string.IsNullOrWhiteSpace(selectedBook.ImagePath))
                    {
                        await _cloudinaryService.DeleteImageAsync(selectedBook.ImagePath);
                    }

                    OnShowMessage?.Invoke("Book deleted successfully!");
                    await LoadDataAsync();
                }
                else
                {
                    OnShowMessage?.Invoke("Book delete failed!");
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
