using BookStore_Management_AppDesktop.Helpers;
using BookStore_Management_AppDesktop.Helpers.Enums;
using BookStore_Management_AppDesktop.Messages;
using BookStore_Management_AppDesktop.Models;
using BookStore_Management_AppDesktop.Models.DTOs.BookDTOs; 
using BookStore_Management_AppDesktop.Services.API.BookServices; 
using BookStore_Management_AppDesktop.Services.Navigation;
using BookStore_Management_AppDesktop.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class BookViewModel : BaseViewModel
    {
        private readonly IBookApiService _apiService;
        private readonly INavigationService _navigationService;
        private readonly DebounceHelper _searchDebouncer = new DebounceHelper();

        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private string? _selectedSort;

        [ObservableProperty]
        private ObservableCollection<Book> _books = new ObservableCollection<Book>();

        [ObservableProperty] private int _currentPage = 1;
        [ObservableProperty] private int _pageSize = 12; 
        [ObservableProperty] private int _totalItems;
        [ObservableProperty] private int _totalPages = 1;

        public BookViewModel(IBookApiService apiService, INavigationService navigationService)
        {
            _apiService = apiService;
            _navigationService = navigationService;

            WeakReferenceMessenger.Default.Register<BookChangedMessage>(this, async (recipient, message) =>
            {
                await Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    await ExecuteSearchAsync();
                });
            });

        }

        public override async Task LoadDataAsync()
        {
            await ExecuteSearchAsync();
        }

        partial void OnSearchTextChanged(string value)
        {
            _ = _searchDebouncer.RunAsync(997, async (token) =>
            {
                CurrentPage = 1; 
                await ExecuteSearchAsync(token);
            });
        }

        partial void OnSelectedSortChanged(string? value)
        {
            CurrentPage = 1; 
            _ = ExecuteSearchAsync();
        }

        private async Task ExecuteSearchAsync(CancellationToken token = default)
        {
            try
            {
                string? sortBy = null;
                string? sortOrder = null;

                if (!string.IsNullOrEmpty(SelectedSort) && SelectedSort.Contains("_"))
                {
                    var parts = SelectedSort.Split('_');
                    sortBy = parts[0];    
                    sortOrder = parts[1]; 
                }

                var query = new BookQueryParameters
                {
                    Keyword = SearchText,
                    SortBy = sortBy,
                    SortOrder = sortOrder,
                    PageNumber = CurrentPage,
                    PageSize = PageSize
                };

                var response = await _apiService.GetAllBooksAsync(query, token);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Books.Clear();

                    if (response != null && response.Data != null)
                    {
                        foreach (var book in response.Data)
                        {
                            Books.Add(book);
                        }

                        TotalItems = response.TotalItems;
                        TotalPages = response.TotalPages > 0 ? response.TotalPages : 1;

                        if (CurrentPage > TotalPages)
                        {
                            CurrentPage = TotalPages;
                            _ = ExecuteSearchAsync();
                        }
                    }
                });
            }
            catch (OperationCanceledException) { }
        }

        [RelayCommand]
        private void ViewBookDetail(Book selectedBook)
        {
            if (selectedBook == null) return;

            _navigationService.NavigateTo(PageType.BookDetail, selectedBook.BookId);
        }
    }
}