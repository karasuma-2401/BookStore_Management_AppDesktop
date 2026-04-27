
using CommunityToolkit.Mvvm.ComponentModel;

namespace BookStore_Management_AppDesktop.Models
{
    public partial class ImportCartItem : ObservableObject
    {
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public int CurrentQuantity { get; set; } 

        [ObservableProperty]
        private int _importQuantity;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ExpectedRetailPrice))] 
        [NotifyPropertyChangedFor(nameof(TotalLinePrice))]
        private decimal _importPrice; 


        public decimal TotalLinePrice => ImportQuantity * ImportPrice;

        public decimal ExpectedRetailPrice => ImportPrice * 1.05m;
    }
}
