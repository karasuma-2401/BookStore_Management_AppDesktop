using BookStore_Management_AppDesktop.Services;
using BookStore_Management_AppDesktop.Services.API;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Net.Http;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly IVoucherApiService _voucherApiService;
        private readonly IDialogService _dialogService;

        [ObservableProperty] private ObservableCollection<VoucherDto> vouchers = new();

        [ObservableProperty] private string? newVoucherCode;

        [ObservableProperty] private int? newVoucherDiscountPercent;

        [ObservableProperty] private decimal? newVoucherDiscountAmount;

        [ObservableProperty] private DateTime? newVoucherExpiryDate = DateTime.UtcNow.AddMonths(1);

        [ObservableProperty] private int? newVoucherUsageLimit;

        [ObservableProperty] private bool isLoading;

        public Action<string>? OnShowMessage { get; set; }

        public SettingsViewModel(IVoucherApiService voucherApiService, IDialogService dialogService)
        {
            _voucherApiService = voucherApiService;
            _dialogService = dialogService;
        }

        public async Task InitializeAsync()
        {
            await LoadVouchersAsync();
        }

      

        [RelayCommand]
        private async Task LoadVouchers()
        {
            await LoadVouchersAsync();
        }

        private async Task LoadVouchersAsync()
        {
            try
            {
                IsLoading = true;
                var voucherList = await _voucherApiService.GetAllVouchersAsync();

                Vouchers.Clear();
                foreach (var voucher in voucherList)
                {
                    Vouchers.Add(voucher);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Error loading vouchers]: {ex.Message}");
                OnShowMessage?.Invoke("Failed to load vouchers");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task CreateVoucher()
        {
            try
            {
                // Validation: Code
                if (string.IsNullOrWhiteSpace(NewVoucherCode))
                {
                    OnShowMessage?.Invoke("❌ Please enter a voucher code");
                    return;
                }

                var codeRegex = new System.Text.RegularExpressions.Regex(@"^[A-Z0-9]+$");
                if (!codeRegex.IsMatch(NewVoucherCode.ToUpper().Trim()))
                {
                    OnShowMessage?.Invoke("❌ Code must only contain uppercase letters and numbers.");
                    return;
                }

                // Validation: Discount
                if (!NewVoucherDiscountPercent.HasValue && !NewVoucherDiscountAmount.HasValue)
                {
                    OnShowMessage?.Invoke("❌ Please specify either a discount percent or minimum spend.");
                    return;
                }

                // Validation: Expiry Date
                if (NewVoucherExpiryDate.HasValue && NewVoucherExpiryDate <= DateTime.Now)
                {
                    OnShowMessage?.Invoke("❌ Expiry date must be in the future.");
                    return;
                }

                // Validation: Discount Percent Range
                if (NewVoucherDiscountPercent.HasValue && (NewVoucherDiscountPercent < 0 || NewVoucherDiscountPercent > 100))
                {
                    OnShowMessage?.Invoke("❌ Discount percent must be between 0 and 100.");
                    return;
                }

                // Validation: Minimum Spend
                if (NewVoucherDiscountAmount.HasValue && NewVoucherDiscountAmount < 0)
                {
                    OnShowMessage?.Invoke("❌ Minimum spend cannot be negative.");
                    return;
                }

                // Validation: Usage Limit
                if (NewVoucherUsageLimit.HasValue && NewVoucherUsageLimit < 0)
                {
                    OnShowMessage?.Invoke("❌ Usage limit cannot be negative.");
                    return;
                }

                IsLoading = true;

                var createDto = new VoucherCreateRequestDto
                {
                    Code = NewVoucherCode.ToUpper().Trim(),
                    DiscountPercent = NewVoucherDiscountPercent,
                    DiscountAmount = NewVoucherDiscountAmount,
                    ExpiryDate = NewVoucherExpiryDate,
                    UsageLimit = NewVoucherUsageLimit
                };

                Debug.WriteLine($"[Creating Voucher] Code: {createDto.Code}, DiscountPercent: {createDto.DiscountPercent}, DiscountAmount: {createDto.DiscountAmount}, ExpiryDate: {createDto.ExpiryDate}, UsageLimit: {createDto.UsageLimit}");

                var result = await _voucherApiService.CreateVoucherAsync(createDto);

                if (result != null)
                {
                    Debug.WriteLine($"[Voucher Created] VoucherId: {result.VoucherId}, Code: {result.Code}");
                    OnShowMessage?.Invoke("✅ Voucher created successfully!");
                    ClearVoucherForm();
                    await LoadVouchersAsync();
                }
                else
                {
                    Debug.WriteLine($"[Error] CreateVoucherAsync returned null result");
                    OnShowMessage?.Invoke("❌ Failed to create voucher: Server returned no data.");
                }
            }
            catch (HttpRequestException hEx)
            {
                Debug.WriteLine($"[HttpError creating voucher] Type: {hEx.GetType().Name} - Message: {hEx.Message}");
                Debug.WriteLine($"[HttpError Details] InnerException: {hEx.InnerException?.Message}");
                OnShowMessage?.Invoke($"❌ Network error: {hEx.Message}");
            }
            catch (System.Text.Json.JsonException jEx)
            {
                Debug.WriteLine($"[JsonError creating voucher] Type: {jEx.GetType().Name} - Message: {jEx.Message}");
                Debug.WriteLine($"[JsonError Details] Path: {jEx.Path}, LineNumber: {jEx.LineNumber}");
                OnShowMessage?.Invoke($"❌ Invalid response format: {jEx.Message}");
            }
            catch (InvalidOperationException opEx)
            {
                Debug.WriteLine($"[OperationError creating voucher] Type: {opEx.GetType().Name} - Message: {opEx.Message}");
                Debug.WriteLine($"[OperationError Stack] {opEx.StackTrace}");
                OnShowMessage?.Invoke($"❌ Operation error: {opEx.Message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Error creating voucher] Type: {ex.GetType().Name}");
                Debug.WriteLine($"[Error Message] {ex.Message}");
                Debug.WriteLine($"[Stack Trace] {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"[InnerException] {ex.InnerException.Message}");
                }
                OnShowMessage?.Invoke($"❌ Unexpected error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task DeleteVoucher(VoucherDto voucher)
        {
            if (voucher == null) return;

            try
            {
                var confirmed = _dialogService.ShowConfirmation(
                    $"Are you sure you want to delete voucher '{voucher.Code}'?",
                    "Delete",
                    true);

                if (!confirmed) return;

                IsLoading = true;
                Debug.WriteLine($"[Deleting Voucher] VoucherId: {voucher.VoucherId}, Code: {voucher.Code}");

                var (success, message) = await _voucherApiService.DeleteVoucherAsync(voucher.VoucherId);

                if (success)
                {
                    Debug.WriteLine($"[Voucher Deleted] Code: {voucher.Code}");
                    OnShowMessage?.Invoke("✅ Voucher deleted successfully!");
                    Vouchers.Remove(voucher);
                }
                else
                {
                    if (message != null && message.Contains("deactivated & expired"))
                    {
                        Debug.WriteLine($"[Voucher Deactivated] Code: {voucher.Code}");
                        OnShowMessage?.Invoke($"⚠️ {message}");
                        await LoadVouchersAsync();
                    }
                    else
                    {
                        Debug.WriteLine($"[Error] Failed to delete voucher {voucher.VoucherId}: {message}");
                        OnShowMessage?.Invoke($"❌ {message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Error deleting voucher]: {ex.GetType().Name} - {ex.Message}");
                OnShowMessage?.Invoke($"❌ Error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ClearVoucherForm()
        {
            NewVoucherCode = null;
            NewVoucherDiscountPercent = null;
            NewVoucherDiscountAmount = null;
            NewVoucherExpiryDate = DateTime.Now.AddMonths(1);
            NewVoucherUsageLimit = null;
        }
    }
}
