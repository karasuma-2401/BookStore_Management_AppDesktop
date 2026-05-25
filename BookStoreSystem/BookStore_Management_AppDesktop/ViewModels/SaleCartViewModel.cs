using BookStore_Management_AppDesktop.Helpers.Enums;
using BookStore_Management_AppDesktop.Models;
using BookStore_Management_AppDesktop.Models.DTOs.CustomerDTOs;
using BookStore_Management_AppDesktop.Models.DTOs.InvoiceDTOs;
using BookStore_Management_AppDesktop.Services;
using BookStore_Management_AppDesktop.Services.API;
using BookStore_Management_AppDesktop.Services.API.CartServices;
using BookStore_Management_AppDesktop.Services.API.InvoiceServices;
using BookStore_Management_AppDesktop.Services.API.CustomerServices;
using BookStore_Management_AppDesktop.Services.Navigation;
using BookStore_Management_AppDesktop.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class SaleCartViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly ICartService _cartService;
        private readonly IInvoiceApiService _invoiceApiService;
        private readonly IDialogService _dialogService;
        private readonly IVoucherApiService _voucherApiService;
        private readonly ICustomerApiService _customerApiService;

        [ObservableProperty]
        private ObservableCollection<SaleCartItem> cartItems;

        [ObservableProperty]
        private decimal totalPrice;

        [ObservableProperty]
        private decimal discountAmount;

        [ObservableProperty]
        private decimal finalTotal;

        [ObservableProperty]
        private int _totalQuantity;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private int? selectedCustomerId;

        [ObservableProperty]
        private string? voucherCodeInput;

        [ObservableProperty]
        private string? appliedVoucherMessage;

        [ObservableProperty]
        private bool _isVoucherValid;

        [ObservableProperty]
        private ObservableCollection<CustomerResponseDto> customers = new();

        public SaleCartViewModel(INavigationService navigationService, ICartService cartService, 
                               IInvoiceApiService invoiceApiService, IDialogService dialogService, IVoucherApiService voucherApiService
                               , ICustomerApiService customerApiService)
        {
            _navigationService = navigationService;
            _cartService = cartService;
            _invoiceApiService = invoiceApiService;
            _dialogService = dialogService;
            _voucherApiService = voucherApiService;
            _customerApiService = customerApiService;

            CartItems = _cartService.GetCartItem();
            TotalPrice = _cartService.TotalPrice;
            FinalTotal = TotalPrice;
            UpdateTotalQuantity();

            CartItems.CollectionChanged += CartItems_CollectionChanged;
            _ = LoadCustomers();
        }

        private async Task LoadCustomers()
        {
            if (_customerApiService == null) return;

            try
            {
                var list = await _customerApiService.GetAllCustomersAsync();
                if (list != null)
                {
                    Customers = new ObservableCollection<CustomerResponseDto>(list);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading customers: {ex.Message}");
            }
        }

        [RelayCommand]
        private void OpenAddCustomerDialog()
        {
            // Cửa sổ này giờ tự gọi API và trả về khách hàng đã tạo thành công
            var createdCustomer = _dialogService.ShowAddCustomerWindow();

            if (createdCustomer != null)
            {
                // Thêm vào danh sách để giao diện cập nhật ngay
                Customers.Add(createdCustomer);
                SelectedCustomerId = createdCustomer.CustomerId;
            }
        }

        private void CartItems_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateTotalQuantity();
            TotalPrice = _cartService.TotalPrice;
            RecalculateFinalTotal();
        }

        private void UpdateTotalQuantity()
        {
            TotalQuantity = CartItems.Sum(x => x.Quantity);
        }

        private void RecalculateFinalTotal()
        {
            FinalTotal = TotalPrice - DiscountAmount;
            if (FinalTotal < 0) FinalTotal = 0;
        }

        partial void OnDiscountAmountChanged(decimal oldValue, decimal newValue)
        {
            RecalculateFinalTotal();
        }

        [RelayCommand]
        private void GoBack()
        {
            _navigationService.NavigateTo(PageType.Books);
        }

        [RelayCommand]
        private void ContinueShopping()
        {
            _navigationService.NavigateTo(PageType.Books);
        }

        [RelayCommand]
        private void RemoveItem(SaleCartItem item)
        {
            if (item != null)
            {
                _cartService.RemoveFromCart(item.BookId);
            }
        }

        [RelayCommand]
        private void IncreaseQuantity(SaleCartItem item)
        {
            if (item != null)
            {
                _cartService.UpdateQuantity(item.BookId, item.Quantity + 1);
                UpdateTotalQuantity();
            }
        }

        [RelayCommand]
        private void DecreaseQuantity(SaleCartItem item)
        {
            if (item != null)
            {
                if (item.Quantity > 1)
                {
                    _cartService.UpdateQuantity(item.BookId, item.Quantity - 1);
                }
                else
                {
                    RemoveItem(item);
                }
                UpdateTotalQuantity();
            }
        }

        [RelayCommand]
        private void ClearCart()
        {
            bool isConfirmed = _dialogService.ShowConfirmation(
                message: "Are you sure you want to clear the cart?",
                confirmText: "Clear",
                isDanger: true);

            if (isConfirmed)
            {
                _cartService.ClearCart();
                UpdateTotalQuantity();
            }
        }

        [RelayCommand]
        private async Task Checkout()
        {
            if (!CartItems.Any())
            {
                _dialogService.ShowMessage("Your cart is empty!");
                return;
            }

            bool isConfirmed = _dialogService.ShowConfirmation(
                message: $"Proceed with checkout?\nTotal: {FinalTotal:#,0} đ",
                confirmText: "Checkout",
                isDanger: false);

            if (!isConfirmed)
                return;

            try
            {
                IsLoading = true;

                var invoiceDto = new InvoiceCreateDto
                {
                    CustomerId = SelectedCustomerId,
                    VoucherCode = VoucherCodeInput,
                    Details = CartItems.Select(item => new InvoiceDetailCreateDto
                    {
                        BookId = item.BookId,
                        Quantity = item.Quantity
                    }).ToList()
                };

                var invoiceId = await _invoiceApiService.CreateInvoiceAsync(invoiceDto);

                if (invoiceId.HasValue)
                {
                    _dialogService.ShowMessage("Checkout successful! Invoice created.");
                    _cartService.ClearCart();
                    UpdateTotalQuantity();
                    DiscountAmount = 0;
                    VoucherCodeInput = null;
                    SelectedCustomerId = null;
                    _navigationService.NavigateTo(PageType.InvoiceDetail, invoiceId.Value);
                }
                else
                {
                    _dialogService.ShowMessage("Failed to create invoice. Please try again.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Error during checkout]: {ex.Message}");
                _dialogService.ShowMessage($"An error occurred: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task ApplyVoucher()
        {
            if (string.IsNullOrWhiteSpace(VoucherCodeInput))
            {
                DiscountAmount = 0;
                IsVoucherValid = false;
                AppliedVoucherMessage = null;
                RecalculateFinalTotal();
                return;
            }

            try
            {
                IsLoading = true;
                var vouchers = await _voucherApiService.GetAllVouchersAsync();
                var voucher = vouchers.FirstOrDefault(v => v.Code.ToLower() == VoucherCodeInput.ToLower());

                if (voucher == null)
                {
                    IsVoucherValid = false;
                    AppliedVoucherMessage = "Voucher code not found!";
                    DiscountAmount = 0;
                    _dialogService.ShowMessage("Voucher code does not exist.");
                    RecalculateFinalTotal();
                    return;
                }

                // Check if voucher has expired
                if (voucher.ExpiryDate.HasValue && voucher.ExpiryDate < DateTime.Now)
                {
                    IsVoucherValid = false;
                    AppliedVoucherMessage = "Voucher has expired!";
                    DiscountAmount = 0;
                    _dialogService.ShowMessage("This voucher code has expired.");
                    RecalculateFinalTotal();
                    return;
                }

                // Check if usage limit reached
                if (voucher.UsageLimit.HasValue && voucher.UsedCount >= voucher.UsageLimit)
                {
                    IsVoucherValid = false;
                    AppliedVoucherMessage = "Voucher usage limit reached!";
                    DiscountAmount = 0;
                    _dialogService.ShowMessage("This voucher has reached its usage limit.");
                    RecalculateFinalTotal();
                    return;
                }

                // Calculate discount
                decimal discount = 0;
                if (voucher.DiscountPercent.HasValue)
                {
                    discount = TotalPrice * (voucher.DiscountPercent.Value / 100m);
                }
                else if (voucher.DiscountAmount.HasValue)
                {
                    discount = voucher.DiscountAmount.Value;
                }

                // Check minimum order amount
                if (voucher.DiscountAmount.HasValue && TotalPrice < voucher.DiscountAmount.Value)
                {
                    IsVoucherValid = false;
                    AppliedVoucherMessage = $"Minimum order must be {voucher.DiscountAmount.Value:#,0}đ";
                    DiscountAmount = 0;
                    _dialogService.ShowMessage($"Order total must be at least {voucher.DiscountAmount.Value:#,0}đ to use this voucher.");
                    RecalculateFinalTotal();
                    return;
                }

                DiscountAmount = Math.Min(discount, TotalPrice); // Discount can't exceed total price
                IsVoucherValid = true;
                AppliedVoucherMessage = $"Discount applied: -{DiscountAmount:#,0}đ";
                _dialogService.ShowMessage($"Voucher applied successfully! Discount: {DiscountAmount:#,0}đ");
                RecalculateFinalTotal();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Error applying voucher]: {ex.Message}");
                IsVoucherValid = false;
                AppliedVoucherMessage = "Error validating voucher";
                DiscountAmount = 0;
                _dialogService.ShowMessage($"Error: {ex.Message}");
                RecalculateFinalTotal();
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}