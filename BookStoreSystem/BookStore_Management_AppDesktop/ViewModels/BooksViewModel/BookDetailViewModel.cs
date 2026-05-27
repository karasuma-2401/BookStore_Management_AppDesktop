using BookStore_Management_AppDesktop.Helpers.Enums;
using BookStore_Management_AppDesktop.Models;
using BookStore_Management_AppDesktop.Models.DTOs.BookDTOs;
using BookStore_Management_AppDesktop.Services.API.BookServices;
using BookStore_Management_AppDesktop.Services.API.CartServices;
using BookStore_Management_AppDesktop.Services.Navigation;
using BookStore_Management_AppDesktop.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Windows;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class BookDetailViewModel : BaseViewModel, INavigatable  
    {
        private readonly INavigationService _navigationService;
        private readonly IBookApiService _apiService;
        private readonly ICartService _cartService;

        [ObservableProperty]
        private Book? _currentBook;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private int _selectedQuantity = 1;

        public int BookId { get; set; }

        public BookDetailViewModel(INavigationService navigationService, IBookApiService apiService, ICartService cartService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
            _cartService = cartService;
        }

        public void OnNavigatedTo(object? parameter)
        {
            if (parameter is int id)
            {
                BookId = id;
                _ = LoadDataAsync();
            }
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
        private void GoBack()
        {
            _navigationService.NavigateTo(PageType.Books);
        }

        [RelayCommand]
        private void IncreaseQuantity()
        {
            SelectedQuantity++;
        }

        [RelayCommand]
        private void DecreaseQuantity()
        {
            if (SelectedQuantity > 1)
            {
                SelectedQuantity--;
            }
        }

        [RelayCommand]
        private void AddToCart()
        {
            if (CurrentBook != null && SelectedQuantity > 0)
            {
                // Convert Book to BookResponseDto for cart service
                var bookDto = new BookResponseDto
                {
                    BookId = CurrentBook.BookId,
                    Title = CurrentBook.Title,
                    Price = CurrentBook.Price,
                    ImagePath = CurrentBook.ImagePath,
                    Quantity = CurrentBook.Quantity
                };

                _cartService.AddToCart(bookDto, SelectedQuantity);
                MessageBox.Show($"Added {SelectedQuantity} copy(ies) of '{CurrentBook.Title}' to cart!", 
                    "Added to Cart", MessageBoxButton.OK, MessageBoxImage.Information);
                SelectedQuantity = 1; // Reset quantity
            }
        }
    }
}
