using BookStore_Management_AppDesktop.Models;
using BookStore_Management_AppDesktop.Services.API.BookServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class CategorySelectionViewModel : ObservableObject
    {
        private readonly IBookApiService _apiService;
        private List<Category> _allCategories = new();

        [ObservableProperty] private ObservableCollection<Category> _displayCategories = new();
        [ObservableProperty] private Category? _selectedCategory;
        [ObservableProperty] private string _searchCategoryText = string.Empty;
        [ObservableProperty] private bool _isShowAddButton;

        public Action<string>? OnShowMessage { get; set; }

        public CategorySelectionViewModel(IBookApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task InitializeAsync(int? initialCategoryId = null)
        {
            var categories = await _apiService.GetAllCategoriesAsync();
            _allCategories = categories.ToList();

            FilterCategories();

            if (initialCategoryId.HasValue)
            {
                SelectedCategory = _allCategories.FirstOrDefault(c => c.CategoryId == initialCategoryId.Value);
            }
        }

        partial void OnSearchCategoryTextChanged(string value)
        {
            FilterCategories();
            IsShowAddButton = !string.IsNullOrWhiteSpace(value) &&
                              !_allCategories.Any(c => c.Name.Equals(value, StringComparison.OrdinalIgnoreCase));
        }

        private void FilterCategories()
        {
            var filtered = string.IsNullOrWhiteSpace(SearchCategoryText)
                ? _allCategories
                : _allCategories.Where(c => c.Name.Contains(SearchCategoryText, StringComparison.OrdinalIgnoreCase)).ToList();

            DisplayCategories = new ObservableCollection<Category>(filtered);
        }

        public List<int> GetSelectedCategoryIds()
        {
            return _allCategories.Where(c => c.IsSelected).Select(c => c.CategoryId).ToList();
        }

        [RelayCommand]
        private async Task AddCategoryAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchCategoryText)) return;

            var newCat = new Category { Name = SearchCategoryText.Trim(), IsSelected = true };
            var created = await _apiService.CreateCategoryAsync(newCat);

            if (created != null)
            {
                created.IsSelected = true;
                _allCategories.Add(created);
                SelectedCategory = created;
                SearchCategoryText = string.Empty; 
                FilterCategories();
                OnShowMessage?.Invoke("New category added!");
            }
        }
    }
}