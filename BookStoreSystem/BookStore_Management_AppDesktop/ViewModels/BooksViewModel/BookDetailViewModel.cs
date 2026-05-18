using BookStore_Management_AppDesktop.Messages; 
using BookStore_Management_AppDesktop.Messages.BookStore_Management_AppDesktop.Messages;
using BookStore_Management_AppDesktop.Models;
using BookStore_Management_AppDesktop.Services.API.BookServices;
using BookStore_Management_AppDesktop.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging; 
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class BookDetailViewModel : BaseViewModel
    {
        private readonly IBookApiService _apiService;

        [ObservableProperty]
        private Book? _currentBook;

        [ObservableProperty]
        private bool _isLoading;

        public int BookId { get; set; }

        public BookDetailViewModel(IBookApiService apiService)
        {
            _apiService = apiService;

            WeakReferenceMessenger.Default.Register<BookSelectedMessage>(this, (recipient, message) =>
            {
                BookId = message.Value; 
                _ = LoadDataAsync();    
            });
        }


        public override async Task LoadDataAsync()
        {
            try
            {
                IsLoading = true;

                var bookDetail = await _apiService.GetBookByIdAsync(BookId);

                if (bookDetail != null)
                {
                    CurrentBook = bookDetail;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Error loading book details]: {ex.Message}");
                MessageBox.Show("Unable to load book details at this time!", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void AddToCart()
        {
            // TODO: Code logic Add book to cart
        }

    }
}