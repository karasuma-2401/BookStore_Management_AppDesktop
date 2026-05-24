using BookStore_Management_AppDesktop.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace BookStore_Management_AppDesktop.Services
{
    public interface ICartService : INotifyPropertyChanged
    {
        ObservableCollection<SaleCartItem> CartItems { get; }
        decimal TotalPrice { get; }
        int ItemCount { get; }

        void AddToCart(Book book, int quantity = 1);
        void RemoveFromCart(SaleCartItem item);
        void ClearCart();
        void UpdateQuantity(SaleCartItem item, int newQuantity);
    }

    public partial class CartService : ObservableObject, ICartService
    {
        [ObservableProperty]
        private ObservableCollection<SaleCartItem> _cartItems = new ObservableCollection<SaleCartItem>();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ItemCount))]
        private decimal _totalPrice;

        public int ItemCount => CartItems.Count;

        public void AddToCart(Book book, int quantity = 1)
        {
            if (quantity <= 0)
                return;

            var existingItem = CartItems.FirstOrDefault(x => x.BookId == book.BookId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var newItem = new SaleCartItem
                {
                    BookId = book.BookId,
                    Title = book.Title ?? string.Empty,
                    AuthorName = book.AuthorName ?? string.Empty,
                    ImagePath = book.ImagePath,
                    Price = book.Price,
                    Quantity = quantity
                };
                newItem.PropertyChanged += CartItem_PropertyChanged;
                CartItems.Add(newItem);
            }

            RecalculateTotalPrice();
        }

        public void RemoveFromCart(SaleCartItem item)
        {
            if (item != null)
            {
                item.PropertyChanged -= CartItem_PropertyChanged;
                CartItems.Remove(item);
                RecalculateTotalPrice();
            }
        }

        public void ClearCart()
        {
            foreach (var item in CartItems)
                item.PropertyChanged -= CartItem_PropertyChanged;

            CartItems.Clear();
            RecalculateTotalPrice();
        }

        public void UpdateQuantity(SaleCartItem item, int newQuantity)
        {
            if (newQuantity <= 0)
            {
                RemoveFromCart(item);
            }
            else
            {
                item.Quantity = newQuantity;
                RecalculateTotalPrice();
            }
        }

        private void CartItem_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SaleCartItem.Quantity) || e.PropertyName == nameof(SaleCartItem.Price))
            {
                RecalculateTotalPrice();
            }
        }

        private void RecalculateTotalPrice()
        {
            TotalPrice = CartItems.Sum(item => item.TotalLinePrice);
        }
    }
}
