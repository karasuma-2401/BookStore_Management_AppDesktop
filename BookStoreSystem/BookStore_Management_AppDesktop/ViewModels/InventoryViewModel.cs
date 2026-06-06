using BookStore_Management_AppDesktop.Helpers;
using BookStore_Management_AppDesktop.Models;
using BookStore_Management_AppDesktop.Models.DTOs.BookDTOs;
using BookStore_Management_AppDesktop.Services;
using BookStore_Management_AppDesktop.Services.API.BookServices;
using BookStore_Management_AppDesktop.Services.Export;
using BookStore_Management_AppDesktop.Services.Realtime;
using BookStore_Management_AppDesktop.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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
        private readonly IServiceProvider _serviceProvider;
        private readonly IExportService _exportService;

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
        [ObservableProperty] private int _lowStockCount = 0;

        public InventoryViewModel(
            IBookApiService apiService,
            CloudinaryService cloudinaryService,
            IDialogService dialogService,
            IBookHubService hubService,
            IServiceProvider serviceProvider,
            IExportService exportService) 
        {
            _apiService = apiService;
            _cloudinaryService = cloudinaryService;
            _dialogService = dialogService;
            _hubService = hubService;
            _serviceProvider = serviceProvider;
            _exportService = exportService;

            _hubService.BookCreated += (b) => RefreshInventoryGrid();
            _hubService.BookDeleted += (id) => RefreshInventoryGrid();
            _hubService.BookUpdated += (id) => RefreshInventoryGrid();
            _hubService.InventoryStockChanged += (id, qty) => RefreshInventoryGrid();
            _hubService.AuthorUpdated += (id, name) => RefreshInventoryGrid();
        }

        private void RefreshInventoryGrid()
        {
            Application.Current.Dispatcher.InvokeAsync(async () => await ExecuteSearchAsync());
        }

        partial void OnSelectedSortChanged(string? value) { CurrentPage = 1; _ = ExecuteSearchAsync(); }
        public override async Task LoadDataAsync() => await ExecuteSearchAsync();

        private async Task ExecuteSearchAsync(CancellationToken token = default)
        {
            try
            {
                string? sortBy = null; string? sortOrder = null;
                if (!string.IsNullOrEmpty(SelectedSort) && SelectedSort.Contains("_"))
                {
                    var parts = SelectedSort.Split('_'); sortBy = parts[0]; sortOrder = parts[1];
                }

                var query = new BookQueryParameters { 
                    Keyword = SearchText?.Trim(), 
                    PageNumber = CurrentPage, 
                    PageSize = PageSize, 
                    SortBy = sortBy, 
                    SortOrder = sortOrder,
                    IncludeOutOfStock = true
                };
                var response = await _apiService.GetAllBooksAsync(query, token);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Books.Clear();
                    if (response != null && response.Data != null)
                    {
                        foreach (var book in response.Data) Books.Add(book);
                        TotalItems = response.TotalItems; 
                        TotalPages = response.TotalPages > 0 ? response.TotalPages : 1;
                        TotalBooksCount = TotalItems.ToString("N0");

                        // Get all books to calculate low stock count across entire dataset
                        _ = UpdateLowStockCountAsync(query, token);

                        if (CurrentPage > TotalPages) { CurrentPage = TotalPages; _ = ExecuteSearchAsync(); }
                    }
                });
            }
            catch (OperationCanceledException) { }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"[Inventory Search Error]: {ex.Message}"); }
        }

        private async Task UpdateLowStockCountAsync(BookQueryParameters query, CancellationToken token)
        {
            try
            {
                // Get all books without pagination to calculate total low stock
                var allBooksQuery = new BookQueryParameters
                {
                    Keyword = query.Keyword,
                    PageNumber = 1,
                    PageSize = 10000, // Get all books
                    IncludeOutOfStock = true
                };

                var response = await _apiService.GetAllBooksAsync(allBooksQuery, token);
                if (response?.Data != null)
                {
                    LowStockCount = response.Data.Count(b => b.Quantity > 0 && b.Quantity <= 5);
                    LowStockBooksCount = LowStockCount.ToString("N0");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Low Stock Update Error]: {ex.Message}");
            }
        }

        partial void OnSearchTextChanged(string value)
        {
            _ = _searchDebouncer.RunAsync(997, async (token) => { CurrentPage = 1; await ExecuteSearchAsync(token); });
        }

        [RelayCommand] private async Task NextPage() { if (CurrentPage < TotalPages) { CurrentPage++; await ExecuteSearchAsync(); } }
        [RelayCommand] private async Task PreviousPage() { if (CurrentPage > 1) { CurrentPage--; await ExecuteSearchAsync(); } }


        [RelayCommand] private void OpenAuthorManagement() => _dialogService.ShowAuthorManagementWindow();
        [RelayCommand] private void OpenCategoryManagement() => _dialogService.ShowCategoryManagementWindow();

        [RelayCommand]
        private async Task ExportInventory()
        {
            try
            {
                if (Books.Count == 0)
                {
                    _dialogService.ShowMessage("No books to export.");
                    return;
                }

                // Get all books for export
                var query = new BookQueryParameters
                {
                    PageNumber = 1,
                    PageSize = 10000,
                    IncludeOutOfStock = true
                };

                var response = await _apiService.GetAllBooksAsync(query);
                if (response?.Data != null && response.Data.Any())
                {
                    await _exportService.ExportInventoryToExcelAsync(response.Data);
                }
                else
                {
                    _dialogService.ShowMessage("Failed to retrieve books for export.");
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessage($"Error exporting inventory: {ex.Message}");
            }
        }

        [RelayCommand]
        private void EditBook(Book selectedBook)
        {
            if (selectedBook == null) return;

            Application.Current.Dispatcher.Invoke(async () =>
            {
                var formVM = _serviceProvider.GetRequiredService<BookFormViewModel>();
                formVM.OnShowMessage = (msg) => _dialogService.ShowMessage(msg);
                formVM.OnSaveSuccess = () => RefreshInventoryGrid();

                await formVM.SetupEditModeAsync(selectedBook); 

                var editWindow = new BookStore_Management_AppDesktop.Views.Windows.EditBookWindow(formVM);
                editWindow.ShowDialog();
            });
        }
    }
}
