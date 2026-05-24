using BookStore_Management_AppDesktop.Helpers.Enums;
using BookStore_Management_AppDesktop.Helpers.Enums;
using BookStore_Management_AppDesktop.Models;
using BookStore_Management_AppDesktop.Models.DTOs.InvoiceDTOs;
using BookStore_Management_AppDesktop.Services;
using BookStore_Management_AppDesktop.Services.API.InvoiceServices;
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

        [ObservableProperty]
        private ObservableCollection<SaleCartItem> cartItems;

        [ObservableProperty]
        private decimal totalPrice;

        [ObservableProperty]
        private int _totalQuantity;

        [ObservableProperty]
        private bool isLoading;

        public SaleCartViewModel(INavigationService navigationService, ICartService cartService, 
                               IInvoiceApiService invoiceApiService, IDialogService dialogService)
        {
            _navigationService = navigationService;
            _cartService = cartService;
            _invoiceApiService = invoiceApiService;
            _dialogService = dialogService;

            CartItems = _cartService.CartItems;
            TotalPrice = _cartService.TotalPrice;
            UpdateTotalQuantity();

            // Subscribe to changes in the cart service
            _cartService.PropertyChanged += CartService_PropertyChanged;
            CartItems.CollectionChanged += CartItems_CollectionChanged;
        }

        private void CartItems_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateTotalQuantity();
        }

        private void CartService_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ICartService.TotalPrice))
            {
                TotalPrice = _cartService.TotalPrice;
            }
        }

        private void UpdateTotalQuantity()
        {
            TotalQuantity = CartItems.Sum(x => x.Quantity);
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
                _cartService.RemoveFromCart(item);
            }
        }

        [RelayCommand]
        private void IncreaseQuantity(SaleCartItem item)
        {
            if (item != null)
            {
                _cartService.UpdateQuantity(item, item.Quantity + 1);
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
                    _cartService.UpdateQuantity(item, item.Quantity - 1);
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
        private async Task CreateInvoice()
        {
            if (!CartItems.Any())
            {
                _dialogService.ShowMessage("Your cart is empty!");
                return;
            }

            bool isConfirmed = _dialogService.ShowConfirmation(
                message: "Create invoice with the items in the cart?",
                confirmText: "Create Invoice",
                isDanger: false);

            if (!isConfirmed)
                return;

            try
            {
                IsLoading = true;

                var invoiceDto = new InvoiceCreateDto
                {
                    CustomerId = null,
                    VoucherCode = null,
                    Details = CartItems.Select(item => new InvoiceDetailCreateDto
                    {
                        BookId = item.BookId,
                        Quantity = item.Quantity
                    }).ToList()
                };

                var invoiceId = await _invoiceApiService.CreateInvoiceAsync(invoiceDto);

                if (invoiceId.HasValue)
                {
                    _dialogService.ShowMessage("Invoice created successfully!");
                    _cartService.ClearCart();
                    UpdateTotalQuantity();
                    _navigationService.NavigateTo(PageType.InvoiceDetail, invoiceId.Value);
                }
                else
                {
                    _dialogService.ShowMessage("Failed to create invoice. Please try again.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Error creating invoice]: {ex.Message}");
                _dialogService.ShowMessage($"An error occurred: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}