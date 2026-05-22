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
        private bool isInvoiceDateSelected = true;

        [ObservableProperty]
        private bool isItemsSelected = false;

        [ObservableProperty]
        private int currentPage = 1;

        [ObservableProperty]
        private int pageSize = 8;

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
        /// LOGIC LỌC DỮ LIỆU ĐÃ ĐƯỢC CHỈNH SỬA:
        /// 1. Tìm kiếm thông minh theo chuỗi ngày hiển thị (dd/MM/yyyy) khi gõ số 4.
        /// 2. XÓA BỎ HOÀN TOÀN TÍNH NĂNG TỰ ĐỘNG SẮP XẾP (SORT), GIỮ NGUYÊN TRẬT TỰ GỐC CỦA DB.
        /// </summary>
        private IEnumerable<InvoiceListDto> FilterData()
        {
            var filtered = _allInvoices.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                string search = SearchText.Trim();

                // TRƯỜNG HỢP 1: Nếu người dùng đang tick chọn Filter là "Items"
                if (SelectedFilter == "Items")
                {
                    if (int.TryParse(search, out int searchNumber))
                    {
                        // Lọc chính xác theo số lượng vật phẩm (Items) có trong hóa đơn
                        filtered = filtered.Where(i => i.TotalItems == searchNumber);
                    }
                    else
                    {
                        // Nếu đang chọn chế độ Items mà cố tình gõ chữ chữ cái -> Trả về trống
                        return Enumerable.Empty<InvoiceListDto>();
                    }
                }
                // TRƯỜNG HỢP 2: Đang để mặc định hoặc các trường thông tin khác
                else
                {
                    filtered = filtered.Where(i =>
                        (i.InvoiceId != null && i.InvoiceId.ToString().Contains(search, StringComparison.OrdinalIgnoreCase))
                        || (i.CustomerName != null && i.CustomerName.Contains(search, StringComparison.OrdinalIgnoreCase))
                        || (i.StaffName != null && i.StaffName.Contains(search, StringComparison.OrdinalIgnoreCase))
                        || (i.Status != null && i.Status.Contains(search, StringComparison.OrdinalIgnoreCase))
                        // Bỏ hoàn toàn .Value để sửa triệt để lỗi CS1061
                        || (i.InvoiceDate.ToString("dd/MM/yyyy").Contains(search, StringComparison.OrdinalIgnoreCase))
                    );
                }
            }

            // Trả trực tiếp danh sách sau khi lọc để bảo toàn trật tự sắp xếp gốc ban đầu của Database/API.
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

            // Làm mới danh sách dựa trên từ khóa tìm kiếm hiện tại với bộ lọc mới, không can thiệp đảo lộn Sort nữa
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
                System.Diagnostics.Debug.WriteLine($"Navigate Error: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task CancelInvoice(InvoiceListDto invoice)
        {
            if (invoice == null) return;

            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa/hủy hóa đơn #{invoice.InvoiceId}?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                bool success = await _apiService.CancelInvoiceAsync(invoice.InvoiceId);
                if (success)
                {
                    MessageBox.Show("Xóa hóa đơn thành công.");
                    await InitializeDataAsync();
                }
                else
                {
                    MessageBox.Show("Xóa hóa đơn thất bại.");
                }
            }
        }
    }
}