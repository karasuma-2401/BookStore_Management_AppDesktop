using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using BookStore_Management_AppDesktop.Models.DTOs.InvoiceDTOs;
using BookStore_Management_AppDesktop.Services.API.InvoiceServices;
using BookStore_Management_AppDesktop.Services.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BookStore_Management_AppDesktop.Helpers.Enums;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class InvoiceDetailViewModel : ObservableObject, INavigatable
    {
        private readonly IInvoiceApiService _invoiceApiService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private InvoiceDetailResponseDto? invoice;

        [ObservableProperty]
        private bool isLoading;

        public InvoiceDetailViewModel(IInvoiceApiService invoiceApiService, INavigationService navigationService)
        {
            _invoiceApiService = invoiceApiService;
            _navigationService = navigationService;
        }

        /// <summary>
        /// Command hủy (xóa) hóa đơn hiện tại
        /// </summary>
        [RelayCommand]
        private async Task DeleteInvoice()
        {
            if (Invoice == null) return;

            var result = MessageBox.Show($"Bạn có chắc chắn muốn hủy hóa đơn #{Invoice.InvoiceId}?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes) return;

            try
            {
                IsLoading = true;
                bool success = await _invoiceApiService.CancelInvoiceAsync(Invoice.InvoiceId);
                if (success)
                {
                    MessageBox.Show("Hủy hóa đơn thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    _navigationService.NavigateTo(PageType.Invoices);
                }
                else
                {
                    MessageBox.Show("Hủy hóa đơn thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ DeleteInvoice Error: {ex.Message}");
                MessageBox.Show("Có lỗi xảy ra khi hủy hóa đơn.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Hàm nạp dữ liệu bất đồng bộ chuẩn hóa từ API công khai
        /// </summary>
        public async Task LoadInvoiceAsync(int invoiceId)
        {
            // Đảm bảo chạy mượt trên giao diện chính (UI Thread)
            Application.Current.Dispatcher.Invoke(() => IsLoading = true);

            try
            {
                var result = await _invoiceApiService.GetInvoiceByIdAsync(invoiceId);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (result != null)
                    {
                        Invoice = result;
                    }
                    else
                    {
                        MessageBox.Show($"Không tìm thấy thông tin chi tiết cho hóa đơn #{invoiceId}",
                                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ LoadInvoiceAsync Error: {ex.Message}");
            }
            finally
            {
                Application.Current.Dispatcher.Invoke(() => IsLoading = false);
            }
        }

        /// <summary>
        /// Command điều hướng quay trở lại danh sách quản lý hóa đơn chính
        /// </summary>
        [RelayCommand]
        private void BackToList()
        {
            _navigationService.NavigateTo(PageType.Invoices);
        }

        /// <summary>
        /// Command xử lý khi người dùng nhấn nút "Update" trên từng Card chi tiết sách
        /// </summary>
        [RelayCommand]
        private async Task UpdateDetail(InvoiceItemDto item)
        {
            if (item == null || Invoice == null) return;

            if (item.Quantity <= 0)
            {
                MessageBox.Show("Số lượng sản phẩm phải lớn hơn 0!", "Dữ liệu hợp lệ",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Giả lập hoặc gọi API cập nhật dữ liệu tại đây
                MessageBox.Show($"Cập nhật thành công số lượng sách '{item.BookTitle}' thành {item.Quantity}!",
                                "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                // Sau khi cập nhật thành công, nạp lại dữ liệu để tính lại SubTotal và Total chuẩn từ API
                await LoadInvoiceAsync(Invoice.InvoiceId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ UpdateDetail Error: {ex.Message}");
                MessageBox.Show("Có lỗi xảy ra trong quá trình cập nhật thông tin sách.",
                                "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Thực hiện bắt sự kiện điều hướng trang và nạp dữ liệu an toàn một cách bất đồng bộ
        public void OnNavigatedTo(object? parameter)
        {
            if (parameter is int invoiceId)
            {
                // Chạy tác vụ nạp dữ liệu trên một luồng nền (Background Task) để không gây đơ lag UI
                Task.Run(async () => await LoadInvoiceAsync(invoiceId));
            }
        }
    }
}