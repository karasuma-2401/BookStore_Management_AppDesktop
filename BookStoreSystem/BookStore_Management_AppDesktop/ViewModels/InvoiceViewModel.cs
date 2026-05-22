using BookStore_Management_AppDesktop.Models.DTOs.InvoiceDTOs;
using BookStore_Management_AppDesktop.Services.API.InvoiceServices;
using BookStore_Management_AppDesktop.Services.Navigation;
using BookStore_Management_AppDesktop.Helpers.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private int currentPage = 1;

        [ObservableProperty]
        private int pageSize = 8;

        public List<int> PageSizeOptions { get; } = new() { 5, 8, 10, 15, 20 };

        public bool IsInvoiceDateSelected => SelectedFilter == "Invoice Date";
        public bool IsTotalAmountSelected => SelectedFilter == "Total Amount";

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
                var data = await _apiService.GetAllInvoicesAsync();
                if (data != null)
                {
                    _allInvoices = data;
                    UpdateDisplayList();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Invoice Load Error: {ex.Message}");
            }
        }

        partial void OnSearchTextChanged(string value)
        {
            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();
            var token = _searchCts.Token;

            // Xử lý Debounce trên UI Thread tránh sinh Worker Thread bất đồng bộ xung đột tài nguyên
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

        private IEnumerable<InvoiceListDto> FilterData()
        {
            var filtered = _allInvoices.Where(i =>
            {
                if (string.IsNullOrWhiteSpace(SearchText)) return true;
                string search = SearchText.ToLower();

                return i.InvoiceId.ToString().Contains(search)
                    || (i.CustomerName?.ToLower().Contains(search) ?? false)
                    || (i.StaffName?.ToLower().Contains(search) ?? false);
            });

            return SelectedFilter switch
            {
                "Total Amount" => filtered.OrderByDescending(i => i.Total),
                _ => filtered.OrderByDescending(i => i.InvoiceDate)
            };
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
            OnPropertyChanged(nameof(IsInvoiceDateSelected));
            OnPropertyChanged(nameof(IsTotalAmountSelected));
            UpdateDisplayList();
        }

        [RelayCommand]
        private void NextPage()
        {
            if (CurrentPage < TotalPages)
                CurrentPage++;
        }

        [RelayCommand]
        private void PreviousPage()
        {
            if (CurrentPage > 1)
                CurrentPage--;
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
                System.Diagnostics.Debug.WriteLine($"Navigate Error: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task CancelInvoice(InvoiceListDto invoice)
        {
            if (invoice == null) return;

            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn hủy hóa đơn #{invoice.InvoiceId}?",
                "Xác nhận",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                bool success = await _apiService.CancelInvoiceAsync(invoice.InvoiceId);
                if (success)
                {
                    MessageBox.Show("Hủy hóa đơn thành công.");
                    await InitializeDataAsync();
                }
                else
                {
                    MessageBox.Show("Hủy hóa đơn thất bại.");
                }
            }
        }
    }
}