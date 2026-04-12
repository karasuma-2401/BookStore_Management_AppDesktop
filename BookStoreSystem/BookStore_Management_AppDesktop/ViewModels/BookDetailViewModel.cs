using BookStore_Management_AppDesktop.Helpers.Enums;
using BookStore_Management_AppDesktop.Models;
using BookStore_Management_AppDesktop.Services.API;
using BookStore_Management_AppDesktop.Services.Navigation;
using BookStore_Management_AppDesktop.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class BookDetailViewModel : BaseViewModel, INavigatable  
    {
        private readonly INavigationService _navigationService;
        private readonly IBookApiService _apiService;

        [ObservableProperty]
        private Book? _currentBook;

        [ObservableProperty]
        private bool _isLoading;

        public int BookId { get; set; }

        public BookDetailViewModel(INavigationService navigationService, IBookApiService apiService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
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
        private void AddToCart()
        {
            // TODO: Code logic Add book to cart
        }
    }
}
