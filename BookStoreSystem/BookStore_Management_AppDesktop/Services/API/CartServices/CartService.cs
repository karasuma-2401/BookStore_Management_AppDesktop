using BookStore_Management_AppDesktop.Models;
using BookStore_Management_AppDesktop.Models.DTOs.BookDTOs;
using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BookStore_Management_AppDesktop.Services.API.CartServices
{
    public class CartService : ICartService
    {
        private readonly ObservableCollection<SaleCartItem> _cartItems = new();
        private readonly HttpClient _httpClient;

        // Sự kiện bắt buộc khi kế thừa INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        public CartService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Tính tổng số lượng tất cả các cuốn sách có trong giỏ hàng
        public int ItemCount => _cartItems.Sum(item => item.Quantity);
        public decimal TotalPrice => _cartItems.Sum(item => item.Quantity * item.Price);


        // Hàm helper kích hoạt thông báo thay đổi lên UI
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<SaleCartItem> GetCartItem()
        {
            return _cartItems;
        }

        public void AddToCart(BookResponseDto book, int quantity = 1)
        {
            if (book == null) return;

            var existingItem = _cartItems.FirstOrDefault(i => i.BookId == book.BookId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                _cartItems.Add(new SaleCartItem
                {
                    BookId = book.BookId,
                    Title = book.Title ?? "Unknown Title",
                    Price = book.Price,
                    ImagePath = book.ImagePath ?? string.Empty,
                    Quantity = quantity
                });
            }

            // BẮT BUỘC: Thông báo số lượng giỏ hàng thay đổi sau khi thêm
            OnPropertyChanged(nameof(ItemCount));
        }

        public void RemoveFromCart(int bookId)
        {
            var item = _cartItems.FirstOrDefault(i => i.BookId == bookId);
            if (item != null)
            {
                _cartItems.Remove(item);

                // BẮT BUỘC: Thông báo số lượng giỏ hàng thay đổi sau khi xóa
                OnPropertyChanged(nameof(ItemCount));
            }
        }

        public void ClearCart()
        {
            _cartItems.Clear();

            // BẮT BUỘC: Thông báo số lượng về 0 sau khi clear giỏ hàng
            OnPropertyChanged(nameof(ItemCount));
        }

        public async Task<bool> CheckoutAsync(int? customerId = null, string voucherCode = null)
        {
            if (_cartItems.Count == 0) return false;

            var checkoutDto = new
            {
                CustomerId = customerId,
                VoucherCode = voucherCode,
                Details = _cartItems.Select(item => new
                {
                    BookId = item.BookId,
                    Quantity = item.Quantity
                }).ToList()
            };

            var response = await _httpClient.PostAsJsonAsync("invoice/checkout", checkoutDto);
            return response.IsSuccessStatusCode;
        }

        public void UpdateQuantity(int bookId, int newQuantity)
        {
            // Tìm cuốn sách trong giỏ hàng theo ID
            var item = _cartItems.FirstOrDefault(i => i.BookId == bookId);

            if (item != null)
            {
                // Cập nhật số lượng
                item.Quantity = newQuantity;

                // Nếu số lượng <= 0 thì xóa luôn khỏi giỏ
                if (item.Quantity <= 0)
                {
                    RemoveFromCart(bookId);
                }
                else
                {
                    // BẮT BUỘC: Thông báo UI cập nhật lại danh sách 
                    // (Đặc biệt quan trọng nếu bạn có binding TotalPrice)
                    OnPropertyChanged(nameof(ItemCount));
                    OnPropertyChanged(nameof(TotalPrice)); // Nếu bạn đã khai báo property này
                }
            }
        }
    }
}