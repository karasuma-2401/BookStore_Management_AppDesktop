using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace BookStore_Management_AppDesktop.Models
{
    public class Book : ObservableObject
    {
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;

        // Author: 
        public List<int> AuthorIds { get; set; } = new();
        public List<string> AuthorNames { get; set; } = new();
        public string DisplayAuthorNames => AuthorNames != null && AuthorNames.Any()
            ? string.Join(", ", AuthorNames)
            : "Unknown Author";

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set
            {
                if (SetProperty(ref _quantity, value))
                {
                    OnPropertyChanged(nameof(StockStatusText));
                    OnPropertyChanged(nameof(StockStatusIcon));
                    OnPropertyChanged(nameof(StockStatusColor));
                    OnPropertyChanged(nameof(IsLowStock));
                }
            }
        }

        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string? ImagePath { get; set; }
        public int PublishYear { get; set; }

        // Category: 
        public List<string> CategoryNames { get; set; } = new();
        public List<int> CategoryIds { get; set; } = new();
        public string DisplayCategoryNames => CategoryNames != null && CategoryNames.Any()
            ? string.Join(", ", CategoryNames)
            : "Uncategorized";

        // Low stock threshold - set externally after loading regulation
        private int _lowStockThreshold = 5;
        public int LowStockThreshold
        {
            get => _lowStockThreshold;
            set
            {
                if (_lowStockThreshold != value)
                {
                    _lowStockThreshold = value;
                    OnPropertyChanged(nameof(StockStatusText));
                    OnPropertyChanged(nameof(StockStatusIcon));
                    OnPropertyChanged(nameof(StockStatusColor));
                    OnPropertyChanged(nameof(IsLowStock));
                }
            }
        }

        // Computed stock display properties
        public bool IsLowStock => Quantity > 0 && Quantity <= LowStockThreshold;
        public bool IsOutOfStock => Quantity == 0;

        public string StockStatusIcon
        {
            get
            {
                if (IsOutOfStock) return "DismissCircle24";
                if (IsLowStock) return "Warning24";
                return "CheckmarkCircle24";
            }
        }

        public string StockStatusText
        {
            get
            {
                if (IsOutOfStock) return "Out of Stock";
                if (IsLowStock && Quantity <= LowStockThreshold) return $"Low Stock ({Quantity} left)";
                return $"{Quantity} In Stock";
            }
        }

        public System.Windows.Media.Color StockStatusColor
        {
            get
            {
                if (IsOutOfStock || IsLowStock)
                    return System.Windows.Media.Color.FromRgb(0xEF, 0x44, 0x44); // Error red
                return System.Windows.Media.Color.FromRgb(0x05, 0x96, 0x69); // Success green
            }
        }
    }
}