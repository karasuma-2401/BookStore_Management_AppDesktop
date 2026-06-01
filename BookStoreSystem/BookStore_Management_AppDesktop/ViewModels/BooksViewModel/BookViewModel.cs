using BookStore_Management_AppDesktop.Helpers;
using BookStore_Management_AppDesktop.Helpers.Enums;
using BookStore_Management_AppDesktop.Models;
using BookStore_Management_AppDesktop.Models.DTOs.BookDTOs;
using BookStore_Management_AppDesktop.Services.API.BookServices;
using BookStore_Management_AppDesktop.Services.API.CartServices;
using BookStore_Management_AppDesktop.Services.Navigation;
using BookStore_Management_AppDesktop.Services.Realtime;
using BookStore_Management_AppDesktop.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class BookViewModel : BaseViewModel
    {
        private readonly IBookApiService _apiService;
        private readonly INavigationService _navigationService;
        private readonly ICartService _cartService;
        private readonly IBookHubService _hubService;
        private readonly DebounceHelper _searchDebouncer = new DebounceHelper();

        [ObservableProperty] private string _searchText = string.Empty;
        [ObservableProperty] private string? _selectedSort;
        [ObservableProperty] private ObservableCollection<Book> _books = new ObservableCollection<Book>();

        [ObservableProperty] private int _currentPage = 1;
        [ObservableProperty] private int _pageSize = 15;
        [ObservableProperty] private int _totalItems;
        [ObservableProperty] private int _totalPages = 1;
        [ObservableProperty] private int _cartItemCount = 0;

        [ObservableProperty] private bool _isDetailPanelOpen;
        [ObservableProperty] private Book? _currentDetailBook;
        [ObservableProperty] private int _selectedQuantity = 1;
        [ObservableProperty] private bool _isDetailLoading;


        public BookViewModel(
            IBookApiService apiService,
            INavigationService navigationService,
            ICartService cartService,
            IBookHubService hubService)
        {
            _apiService = apiService;
            _navigationService = navigationService;
            _cartService = cartService;
            _hubService = hubService;

            _cartService.PropertyChanged += CartService_PropertyChanged;

            _hubService.BookCreated += OnBookRealtimeChanged;
            _hubService.BookDeleted += OnBookIdRealtimeChanged;
            _hubService.BookUpdated += OnBookRealtimeUpdated;
            _hubService.InventoryStockChanged += OnStockRealtimeChanged;
        }

        #region --- HÀM XỬ LÝ REALTIME SIGNALR ---
        private void OnBookRealtimeChanged(Book book) => TriggerRefresh();
        private void OnBookIdRealtimeChanged(int bookId)
        {
            if (CurrentDetailBook?.BookId == bookId)
            {
                Application.Current.Dispatcher.Invoke(() => CloseDetailPanel());
            }
            TriggerRefresh();
        }
        private void OnBookRealtimeUpdated(int bookId)
        {
            if (IsDetailPanelOpen && CurrentDetailBook?.BookId == bookId)
            {
                Application.Current.Dispatcher.InvokeAsync(async () => await RefreshPanelDetailAsync(bookId));
            }
            TriggerRefresh();
        }
        private void OnStockRealtimeChanged(int bookId, int newQuantity)
        {
            if (IsDetailPanelOpen && CurrentDetailBook?.BookId == bookId)
            {
                Application.Current.Dispatcher.Invoke(() => {
                    if (CurrentDetailBook != null) CurrentDetailBook.Quantity = newQuantity;
                });
            }
            TriggerRefresh();
        }

        private void TriggerRefresh()
        {
            Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                await ExecuteSearchAsync();
            });
        }
        #endregion

        private void CartService_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ICartService.ItemCount))
            {
                CartItemCount = _cartService.ItemCount;
            }
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
                    Keyword = SearchText?.Trim(),

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
        private async Task ViewBookDetailAsync(Book selectedBook)
        {
            if (selectedBook == null) return;

            SelectedQuantity = 1;
            IsDetailPanelOpen = true;

            await RefreshPanelDetailAsync(selectedBook.BookId);
        }

        private async Task RefreshPanelDetailAsync(int bookId)
        {
            try
            {
                IsDetailLoading = true;
                var bookDetail = await _apiService.GetBookByIdAsync(bookId);
                if (bookDetail != null)
                {
                    CurrentDetailBook = bookDetail;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Error loading side-panel details]: {ex.Message}");
            }
            finally
            {
                IsDetailLoading = false;
            }
        }

        [RelayCommand]
        private void CloseDetailPanel()
        {
            IsDetailPanelOpen = false;
            CurrentDetailBook = null;
        }

        [RelayCommand]
        private void IncreaseQuantity() => SelectedQuantity++;

        [RelayCommand]
        private void DecreaseQuantity()
        {
            if (SelectedQuantity > 1) SelectedQuantity--;
        }

        [RelayCommand]
        private void AddToCart()
        {
            if (CurrentDetailBook != null && SelectedQuantity > 0)
            {
                var bookDto = new BookResponseDto
                {
                    BookId = CurrentDetailBook.BookId,
                    Title = CurrentDetailBook.Title,
                    Price = CurrentDetailBook.Price,
                    ImagePath = CurrentDetailBook.ImagePath,
                    Quantity = CurrentDetailBook.Quantity
                };

                _cartService.AddToCart(bookDto, SelectedQuantity);
                MessageBox.Show($"Added {SelectedQuantity} copy(ies) of '{CurrentDetailBook.Title}' to cart!",
                    "Added to Cart", MessageBoxButton.OK, MessageBoxImage.Information);
                SelectedQuantity = 1;
            }
        }

        [RelayCommand]
        private void ViewCart()
        {
            _navigationService.NavigateTo(PageType.SaleCart);
        }

        [RelayCommand]
        private async Task NextPageAsync()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                await ExecuteSearchAsync();
            }
        }

        [RelayCommand]
        private async Task PreviousPageAsync()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                await ExecuteSearchAsync();
            }
        }
    }
}