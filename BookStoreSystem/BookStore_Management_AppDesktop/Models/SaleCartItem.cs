using CommunityToolkit.Mvvm.ComponentModel;

namespace BookStore_Management_AppDesktop.Models
{
    public partial class SaleCartItem : ObservableObject
    {
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public string? ImagePath { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TotalLinePrice))]
        private int _quantity = 1;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TotalLinePrice))]
        private decimal _price;

        public decimal TotalLinePrice => _quantity * _price;
    }
}
