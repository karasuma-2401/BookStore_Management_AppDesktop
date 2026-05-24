using BookStore_Management_AppDesktop.Helpers.Enums;
using BookStore_Management_AppDesktop.Models.DTOs.InvoiceDTOs;
using BookStore_Management_AppDesktop.Services.API;
using BookStore_Management_AppDesktop.Services.API.InvoiceServices;
using BookStore_Management_AppDesktop.Services.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class InvoiceViewModel : ObservableObject
    {
        private readonly IInvoiceApiService _apiService;
        private readonly INavigationService _navigationService;
        private List<InvoiceListDto> _allInvoices = new();
        private CancellationTokenSource? _searchCts;

        [ObservableProperty]
        private ObservableCollection<InvoiceListDto> invoices = new();

        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private string selectedFilter = "Invoice Date";

        [ObservableProperty]
        private bool isInvoiceDateSelected = true;

        [ObservableProperty]
        private bool isItemsSelected = false;

        [ObservableProperty]
        private int currentPage = 1;

        [ObservableProperty]
        private int pageSize = 8;

        [ObservableProperty]
        private bool isLoading;

        public List<int> PageSizeOptions { get; } = new() { 5, 8, 10, 15, 20 };

        public int TotalInvoices => FilterData().Count();
        public int TotalPages => Math.Max(1, (int)Math.Ceiling((double)TotalInvoices / PageSize));
        public int CurrentPageStart => TotalInvoices == 0 ? 0 : (CurrentPage - 1) * PageSize + 1;
        public int CurrentPageEnd => Math.Min(CurrentPage * PageSize, TotalInvoices);

        public InvoiceViewModel(IInvoiceApiService apiService, INavigationService navigationService)
        {
            _apiService = apiService;
            _navigationService = navigationService;
        }

        public async Task InitializeDataAsync()
        {
            try
            {
                IsLoading = true;
                var data = await _apiService.GetAllInvoicesAsync();
                if (data != null)
                {
                    _allInvoices = data;
                    UpdateDisplayList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Invoice Load Error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        partial void OnSearchTextChanged(string value)
        {
            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();
            var token = _searchCts.Token;
            _ = UpdateSearchWithDebounceAsync(token);
        }

        private async Task UpdateSearchWithDebounceAsync(CancellationToken token)
        {
            try
            {
                await Task.Delay(400, token);
                if (!token.IsCancellationRequested)
                {
                    CurrentPage = 1;
                    UpdateDisplayList();
                }
            }
            catch (OperationCanceledException) { }
        }

        partial void OnPageSizeChanged(int value)
        {
            CurrentPage = 1;
            UpdateDisplayList();
        }

        partial void OnCurrentPageChanged(int value) => UpdateDisplayList();

        /// <summary>
        /// DATA FILTERING LOGIC:
        /// 1. Intelligent search by display date string format (dd/MM/yyyy).
        /// 2. Preserves the original sorting order retrieved from the database/API.
        /// </summary>
        private IEnumerable<InvoiceListDto> FilterData()
        {
            var filtered = _allInvoices.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                string search = SearchText.Trim();

                // CASE 1: Filter criteria is selected as "Items"
                if (SelectedFilter == "Items")
                {
                    if (int.TryParse(search, out int searchNumber))
                    {
                        // Filter precisely by the total number of items in the invoice
                        filtered = filtered.Where(i => i.TotalItems == searchNumber);
                    }
                    else
                    {
                        // Return empty list if text is entered while in numeric mode
                        return Enumerable.Empty<InvoiceListDto>();
                    }
                }
                // CASE 2: Default filter or general keywords matching text properties
                else
                {
                    filtered = filtered.Where(i =>
                        (i.InvoiceId != null && i.InvoiceId.ToString().Contains(search, StringComparison.OrdinalIgnoreCase))
                        || (i.CustomerName != null && i.CustomerName.Contains(search, StringComparison.OrdinalIgnoreCase))
                        || (i.StaffName != null && i.StaffName.Contains(search, StringComparison.OrdinalIgnoreCase))
                        || (i.Status != null && i.Status.Contains(search, StringComparison.OrdinalIgnoreCase))
                        || (i.InvoiceDate.ToString("dd/MM/yyyy").Contains(search, StringComparison.OrdinalIgnoreCase))
                    );
                }
            }

            return filtered;
        }

        private void UpdateDisplayList()
        {
            var filteredList = FilterData().ToList();
            int totalPages = Math.Max(1, (int)Math.Ceiling((double)filteredList.Count / PageSize));

            Application.Current.Dispatcher.Invoke(() =>
            {
                if (CurrentPage > totalPages) CurrentPage = totalPages;

                var itemsToShow = filteredList
                    .Skip((CurrentPage - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();

                Invoices.Clear();
                foreach (var item in itemsToShow)
                {
                    Invoices.Add(item);
                }

                OnPropertyChanged(nameof(TotalInvoices));
                OnPropertyChanged(nameof(TotalPages));
                OnPropertyChanged(nameof(CurrentPageStart));
                OnPropertyChanged(nameof(CurrentPageEnd));
            });
        }

        [RelayCommand]
        private void ChangeFilter(string filterType)
        {
            SelectedFilter = filterType;

            IsInvoiceDateSelected = (filterType == "Invoice Date");
            IsItemsSelected = (filterType == "Items");

            CurrentPage = 1;
            UpdateDisplayList();
        }

        [RelayCommand]
        private void NextPage()
        {
            if (CurrentPage < TotalPages) CurrentPage++;
        }

        [RelayCommand]
        private void PreviousPage()
        {
            if (CurrentPage > 1) CurrentPage--;
        }

        [RelayCommand]
        private async Task ViewInvoiceDetail(InvoiceListDto invoice)
        {
            if (invoice == null) return;
            try
            {
                _navigationService.NavigateTo(PageType.InvoiceDetail, invoice.InvoiceId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Navigate Error: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task CancelInvoice(InvoiceListDto invoice)
        {
            if (invoice == null) return;

            // Display confirmation dialog prior to execution
            var result = MessageBox.Show($"Are you sure you want to cancel invoice #{invoice.InvoiceId}?",
                                         "Confirm Cancellation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    IsLoading = true;

                    // Execute request against backend service endpoint
                    bool isCanceled = await _apiService.CancelInvoiceAsync(invoice.InvoiceId);

                    if (isCanceled)
                    {
                        MessageBox.Show("Invoice canceled successfully! Inventory stock levels have been restored.",
                                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Reload data records from server database to dynamically update state on the UI grid
                        await InitializeDataAsync();
                    }
                    else
                    {
                        MessageBox.Show("Failed to cancel invoice. It may not exist or has already been canceled.",
                                        "Notification", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"CancelInvoice ViewModel Error: {ex.Message}");
                    MessageBox.Show("A system error occurred while attempting to cancel the invoice.",
                                    "System Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    IsLoading = false;
                }
            }
        }
    }
}