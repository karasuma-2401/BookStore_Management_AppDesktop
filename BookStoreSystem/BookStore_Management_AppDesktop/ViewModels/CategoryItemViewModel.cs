using BookStore_Management_AppDesktop.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class CategoryItemViewModel : ObservableObject
    {
        public Category Category { get; }

        [ObservableProperty]
        private bool _isSelected;

        public CategoryItemViewModel(Category category, bool isSelected = false)
        {
            Category = category;
            IsSelected = isSelected;
        }
    }
}