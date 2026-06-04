using BookStore_Management_AppDesktop.Helpers;
using BookStore_Management_AppDesktop.Models;
using BookStore_Management_AppDesktop.Models.DTOs.BookDTOs;
using BookStore_Management_AppDesktop.Services;
using BookStore_Management_AppDesktop.Services.API.BookServices;
using BookStore_Management_AppDesktop.Services.Realtime;
using BookStore_Management_AppDesktop.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class InventoryViewModel : BaseViewModel
    {
        private readonly IBookApiService _apiService;
        private readonly CloudinaryService _cloudinaryService;
        private readonly IDialogService _dialogService;
        private readonly IBookHubService _hubService;

        private readonly DebounceHelper _searchDebouncer = new DebounceHelper();

        [ObservableProperty] private ObservableCollection<Book> _books = new ObservableCollection<Book>();
        [ObservableProperty] private string _searchText = string.Empty;

        [ObservableProperty] private int _currentPage = 1;
        [ObservableProperty] private int _pageSize = 12;
        [ObservableProperty] private int _totalItems;
        [ObservableProperty] private int _totalPages = 1;

        [ObservableProperty] private string? _selectedSort = "price_desc";

        [ObservableProperty] private string _totalBooksCount = "0";
        [ObservableProperty] private string _lowStockBooksCount = "0";

        public InventoryViewModel(
            IBookApiService apiService,
            CloudinaryService cloudinaryService,
            IDialogService dialogService,
            IBookHubService hubService)
        {
            _apiService = apiService;
            _cloudinaryService = cloudinaryService;
            _dialogService = dialogService;
            _hubService = hubService;

            _hubService.BookCreated += (b) => RefreshInventoryGrid();
            _hubService.BookDeleted += (id) => RefreshInventoryGrid();
            _hubService.BookUpdated += (id) => RefreshInventoryGrid();
            _hubService.InventoryStockChanged += (id, qty) => RefreshInventoryGrid();
            _hubService.AuthorUpdated += (id, name) => RefreshInventoryGrid();
        }

        private void RefreshInventoryGrid()
        {
            Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                await ExecuteSearchAsync();
            });
        }

        partial void OnSelectedSortChanged(string? value)
        {
            CurrentPage = 1;
            _ = ExecuteSearchAsync();
        }

        public override async Task LoadDataAsync()
        {
            await ExecuteSearchAsync();
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
                    Keyword = SearchText?.Trim(),
                    PageNumber = CurrentPage,
                    PageSize = PageSize,
                    SortBy = sortBy,
                    SortOrder = sortOrder
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

                        TotalBooksCount = TotalItems.ToString("N0");

                        int lowStockCount = Books.Count(b => b.Quantity <= 2);
                        LowStockBooksCount = lowStockCount.ToString("N0");

                        if (CurrentPage > TotalPages)
                        {
                            CurrentPage = TotalPages;
                            _ = ExecuteSearchAsync();
                        }
                    }
                });
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Inventory Search Error]: {ex.Message}");
            }
        }

        partial void OnSearchTextChanged(string value)
        {
            _ = _searchDebouncer.RunAsync(997, async (token) =>
            {
                CurrentPage = 1;
                await ExecuteSearchAsync(token);
            });
        }

        [RelayCommand]
        private async Task NextPage()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                await ExecuteSearchAsync();
            }
        }

        [RelayCommand]
        private async Task PreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                await ExecuteSearchAsync();
            }
        }

        [RelayCommand]
        private async Task DeleteBookAsync(Book selectedBook)
        {
            if (selectedBook == null) return;

            bool isConfirmed = _dialogService.ShowConfirmation(
                                                        message: "Do you want to delete this book?",
                                                        confirmText: "Delete Book",
                                                        isDanger: true);

            if (isConfirmed)
            {
                bool isSuccess = await _apiService.DeleteBookAsync(selectedBook.BookId);

                if (isSuccess)
                {
                    if (!string.IsNullOrWhiteSpace(selectedBook.ImagePath))
                    {
                        await _cloudinaryService.DeleteImageAsync(selectedBook.ImagePath);
                    }

                    _dialogService.ShowMessage("Book deleted successfully!");
                }
                else
                {
                    _dialogService.ShowMessage("Book delete failed!");
                }
            }
        }
        [RelayCommand]
        private void OpenAuthorManagement()
        {
            _dialogService.ShowAuthorManagementWindow();
        }

        [RelayCommand]
        private void OpenCategoryManagement()
        {
            _dialogService.ShowCategoryManagementWindow();
        }


        [RelayCommand]
        private void EditBook(Book selectedBook)
        {
            if (selectedBook == null) return;
            _dialogService.ShowEditBookWindow(selectedBook);
        }
    }
}