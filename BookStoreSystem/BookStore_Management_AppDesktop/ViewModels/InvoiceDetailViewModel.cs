using BookStore_Management_AppDesktop.Helpers.Enums;
using BookStore_Management_AppDesktop.Models.DTOs.InvoiceDTOs;
using BookStore_Management_AppDesktop.Services.API;
using BookStore_Management_AppDesktop.Services.API.InvoiceServices;
using BookStore_Management_AppDesktop.Services.Export; // Thêm namespace chứa IInvoiceExportService
using BookStore_Management_AppDesktop.Services.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class InvoiceDetailViewModel : ObservableObject, INavigatable
    {
        private readonly IInvoiceApiService _invoiceApiService;
        private readonly INavigationService _navigationService;
        private readonly IInvoiceExportService _invoiceExportService; // Inject service mới vào đây

        [ObservableProperty]
        private InvoiceDetailResponseDto? invoice;

        [ObservableProperty]
        private ObservableCollection<PaymentResponseDto> paymentHistory = new();

        [ObservableProperty]
        private bool isHistoryExpanded = false;

        public string ToggleHistoryButtonText => IsHistoryExpanded ? "Hide Payment History" : "Show Payment History";

        partial void OnIsHistoryExpandedChanged(bool value)
        {
            OnPropertyChanged(nameof(ToggleHistoryButtonText));
        }

        [RelayCommand]
        private void ToggleHistory()
        {
            IsHistoryExpanded = !IsHistoryExpanded;
        }

        partial void OnInvoiceChanged(InvoiceDetailResponseDto? value)
        {
            IsHistoryExpanded = false;
            OnPropertyChanged(nameof(IsPaymentButtonVisible));
            OnPropertyChanged(nameof(IsPaymentHistoryVisible));
        }

        public bool IsPaymentButtonVisible => Invoice != null && Invoice.CustomerId != null && Invoice.CustomerId > 0 && Invoice.RemainingAmount > 0;
        public bool IsPaymentHistoryVisible => Invoice != null && Invoice.CustomerId != null && Invoice.CustomerId > 0;

        [ObservableProperty]
        private bool isLoading;

        public InvoiceDetailViewModel(
            IInvoiceApiService invoiceApiService,
            INavigationService navigationService,
            IInvoiceExportService invoiceExportService) // Thêm tham số vào Constructor
        {
            _invoiceApiService = invoiceApiService;
            _navigationService = navigationService;
            _invoiceExportService = invoiceExportService;
        }

        /// <summary>
        /// Asynchronously exports the active invoice document to an Excel spreadsheet destination.
        /// </summary>
        [RelayCommand]
        private async Task ExportInvoice() // Sửa từ void sang async Task để đồng bộ với ClosedXML Service
        {
            if (Invoice == null)
            {
                MessageBox.Show("Invoice data is still loading or not found.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                IsLoading = true; // Bật loading để chặn user thao tác quậy phá UI lúc đang sinh file Excel

                // Gọi hàm bất đồng bộ từ Service mới xây dựng của bạn
                bool isExported = await _invoiceExportService.ExportInvoiceToExcelAsync(Invoice);

                if (isExported)
                {
                    Debug.WriteLine($"Invoice #{Invoice.InvoiceId} successfully generated.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ExportInvoice Error: {ex.Message}");
                MessageBox.Show($"An error occurred while exporting the invoice: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task ExportPaymentHistory()
        {
            if (Invoice == null)
            {
                MessageBox.Show("Invoice data is still loading or not found.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                IsLoading = true;
                var paymentList = new System.Collections.Generic.List<PaymentResponseDto>(PaymentHistory);
                bool isExported = await _invoiceExportService.ExportPaymentHistoryToExcelAsync(Invoice, paymentList);
                if (isExported)
                {
                    Debug.WriteLine($"Payment History for Invoice #{Invoice.InvoiceId} successfully generated.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ExportPaymentHistory Error: {ex.Message}");
                MessageBox.Show($"An error occurred while exporting payment history: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Command to cancel (delete) the currently viewed invoice structure.
        /// </summary>
        [RelayCommand]
        private async Task DeleteInvoice()
        {
            if (Invoice == null) return;

            var result = MessageBox.Show($"Are you sure you want to cancel invoice #{Invoice.InvoiceId}?",
                                         "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes) return;

            try
            {
                IsLoading = true;
                bool success = await _invoiceApiService.CancelInvoiceAsync(Invoice.InvoiceId);
                if (success)
                {
                    MessageBox.Show("Invoice canceled successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    _navigationService.NavigateTo(PageType.Invoices);
                }
                else
                {
                    MessageBox.Show("Failed to cancel invoice.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DeleteInvoice Error: {ex.Message}");
                MessageBox.Show("An error occurred while canceling the invoice.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task RecordPayment()
        {
            if (Invoice == null) return;

            // Run on UI thread because it shows a Window dialog
            await Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                var dialog = new BookStore_Management_AppDesktop.Views.Windows.PaymentDialog(Invoice.RemainingAmount);
                dialog.Owner = Application.Current.MainWindow;
                if (dialog.ShowDialog() == true)
                {
                    var amount = dialog.PaymentAmount;
                    if (amount > Invoice.RemainingAmount)
                    {
                        MessageBox.Show($"The payment amount ({amount:N0}đ) cannot exceed the remaining due ({Invoice.RemainingAmount:N0}đ).", "Invalid Amount", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    try
                    {
                        IsLoading = true;
                        bool success = await _invoiceApiService.RecordPaymentAsync(Invoice.InvoiceId, amount);
                        if (success)
                        {
                            MessageBox.Show("Payment recorded successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            // Reload invoice data
                            await LoadInvoiceAsync(Invoice.InvoiceId);
                        }
                        else
                        {
                            MessageBox.Show("Failed to record payment.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"RecordPayment Error: {ex.Message}");
                        MessageBox.Show("An error occurred while saving payment: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    finally
                    {
                        IsLoading = false;
                    }
                }
            });
        }

        /// <summary>
        /// Asynchronously fetches and loads invoice data records from the API infrastructure safely.
        /// </summary>
        public async Task LoadInvoiceAsync(int invoiceId)
        {
            // Tối ưu: Loại bỏ bớt Dispatcher.Invoke không cần thiết giúp code gọn gàng, chạy mượt hơn
            IsLoading = true;

            try
            {
                var result = await _invoiceApiService.GetInvoiceByIdAsync(invoiceId);

                if (result != null)
                {
                    Invoice = result;
                    if (result.CustomerId != null && result.CustomerId > 0)
                    {
                        var payments = await _invoiceApiService.GetPaymentsByInvoiceIdAsync(invoiceId);
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            PaymentHistory = new ObservableCollection<PaymentResponseDto>(payments);
                            OnPropertyChanged(nameof(IsPaymentButtonVisible));
                            OnPropertyChanged(nameof(IsPaymentHistoryVisible));
                        });
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            PaymentHistory = new ObservableCollection<PaymentResponseDto>();
                            OnPropertyChanged(nameof(IsPaymentButtonVisible));
                            OnPropertyChanged(nameof(IsPaymentHistoryVisible));
                        });
                    }
                }
                else
                {
                    MessageBox.Show($"Could not find detailed records for invoice #{invoiceId}.",
                                    "Notification", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadInvoiceAsync Error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Command to navigate back to the primary invoice list management page view.
        /// </summary>
        [RelayCommand]
        private void BackToList()
        {
            _navigationService.NavigateTo(PageType.Invoices);
        }

        /// <summary>
        /// Handles page navigation initialization and safely loads context data asynchronously.
        /// </summary>
        public void OnNavigatedTo(object? parameter)
        {
            if (parameter is int invoiceId)
            {
                // Giữ nguyên cơ chế đẩy việc gọi API xuống luồng chạy nền background task worker thread
                Task.Run(async () => await LoadInvoiceAsync(invoiceId));
            }
        }
    }
}