using System.ComponentModel;
using System.Collections.ObjectModel;
using BookStore_Management_AppDesktop.Models;
using BookStore_Management_AppDesktop.Models.DTOs.BookDTOs;

namespace BookStore_Management_AppDesktop.Services.API.CartServices
{
    // Kế thừa INotifyPropertyChanged để hỗ trợ phát sự kiện thay đổi
    public interface ICartService : INotifyPropertyChanged
    {
        int ItemCount { get; } // Thuộc tính lưu tổng số lượng item trong giỏ
        ObservableCollection<SaleCartItem> GetCartItem();
        void AddToCart(BookResponseDto book, int quantity = 1);
        void RemoveFromCart(int bookId);
        void ClearCart();
    }
}