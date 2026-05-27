using BookStore_Management_AppDesktop.Services.API;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly IVoucherApiService _voucherApiService;

        [ObservableProperty] private ObservableCollection<VoucherDto> vouchers = new();

        [ObservableProperty] private string? newVoucherCode;

        [ObservableProperty] private int? newVoucherDiscountPercent;

        [ObservableProperty] private decimal? newVoucherDiscountAmount;

        [ObservableProperty] private DateTime? newVoucherExpiryDate = DateTime.UtcNow.AddMonths(1);

        [ObservableProperty] private int? newVoucherUsageLimit;

        [ObservableProperty] private bool isLoading;

        public Action<string>? OnShowMessage { get; set; }

        public SettingsViewModel(IVoucherApiService voucherApiService)
        {
            _voucherApiService = voucherApiService;
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
            if (string.IsNullOrWhiteSpace(NewVoucherCode))
            {
                OnShowMessage?.Invoke("Voucher code is required.");
                return;
            }

            var codeRegex = new System.Text.RegularExpressions.Regex(@"^[A-Z0-9]+$");
            if (!codeRegex.IsMatch(NewVoucherCode.ToUpper().Trim()))
            {
                OnShowMessage?.Invoke("Code must only contain uppercase letters and numbers.");
                return;
            }

            if (!NewVoucherDiscountPercent.HasValue && !NewVoucherDiscountAmount.HasValue)
            {
                OnShowMessage?.Invoke("Please specify either a discount percent or amount.");
                return;
            }

            if (NewVoucherExpiryDate.HasValue && NewVoucherExpiryDate <= DateTime.Now)
            {
                OnShowMessage?.Invoke("Expiry date must be in the future.");
                return;
            }

            try
            {
                IsLoading = true;

                var createDto = new VoucherCreateRequestDto
                {
                    Code = NewVoucherCode.ToUpper().Trim(),
                    DiscountPercent = NewVoucherDiscountPercent,
                    DiscountAmount = NewVoucherDiscountAmount,
                    ExpiryDate = NewVoucherExpiryDate,
                    UsageLimit = NewVoucherUsageLimit
                };

                var result = await _voucherApiService.CreateVoucherAsync(createDto);

                if (result != null)
                {
                    OnShowMessage?.Invoke("Voucher created successfully!");
                    ClearVoucherForm();
                    await LoadVouchersAsync();
                }
                else
                {
                    OnShowMessage?.Invoke("Failed to create voucher. Please check your data.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Error creating voucher]: {ex.Message}");
                OnShowMessage?.Invoke($"Error: {ex.Message}");
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
                IsLoading = true;
                var result = await _voucherApiService.DeleteVoucherAsync(voucher.VoucherId);

                if (result)
                {
                    OnShowMessage?.Invoke("Voucher deleted successfully!");
                    Vouchers.Remove(voucher);
                }
                else
                {
                    OnShowMessage?.Invoke("Failed to delete voucher");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Error deleting voucher]: {ex.Message}");
                OnShowMessage?.Invoke($"Error: {ex.Message}");
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
