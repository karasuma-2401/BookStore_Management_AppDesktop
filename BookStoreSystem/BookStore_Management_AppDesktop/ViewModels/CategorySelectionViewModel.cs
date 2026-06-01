using BookStore_Management_AppDesktop.Models;
using BookStore_Management_AppDesktop.Services.API.CategoryServices;
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
        private readonly ICategoryApiService _categoryApiService;

        private List<Category> _allCategories = new();

        public ObservableCollection<Category> SelectedCategories { get; } = new();

        public ObservableCollection<Category> SuggestedCategories { get; } = new();

        [ObservableProperty] private string _searchText = string.Empty;
        [ObservableProperty] private bool _isDropdownOpen;
        [ObservableProperty] private bool _canAddNewCategory; 

        public Action<string>? OnShowMessage { get; set; }

        public CategorySelectionViewModel(ICategoryApiService categoryApiService)
        {
            _categoryApiService = categoryApiService;
        }

        public async Task InitializeAsync(List<int>? selectedCategoryIds = null)
        {
            SelectedCategories.Clear();
            _allCategories.Clear();

            var fetchedCategories = await _categoryApiService.GetAllCategoriesAsync();
            if (fetchedCategories != null)
            {
                _allCategories = fetchedCategories;
            }

            if (selectedCategoryIds != null && selectedCategoryIds.Any())
            {
                foreach (var id in selectedCategoryIds)
                {
                    var cat = _allCategories.FirstOrDefault(c => c.CategoryId == id);
                    if (cat != null) SelectedCategories.Add(cat);
                }
            }
        }

        partial void OnSearchTextChanged(string value)
        {
            SuggestedCategories.Clear();
            CanAddNewCategory = false;

            if (string.IsNullOrWhiteSpace(value))
            {
                IsDropdownOpen = false;
                return;
            }

            string keyword = value.Trim().ToLower();

            var matches = _allCategories
                .Where(c => c.Name.ToLower().Contains(keyword) && !SelectedCategories.Any(sc => sc.CategoryId == c.CategoryId))
                .ToList();

            foreach (var match in matches)
            {
                SuggestedCategories.Add(match);
            }

            bool exactMatchExists = _allCategories.Any(c => c.Name.Equals(value.Trim(), StringComparison.OrdinalIgnoreCase));
            if (!exactMatchExists)
            {
                CanAddNewCategory = true;
            }

            IsDropdownOpen = SuggestedCategories.Any() || CanAddNewCategory;
        }

        [RelayCommand]
        private void SelectCategory(Category category)
        {
            if (category == null || SelectedCategories.Any(c => c.CategoryId == category.CategoryId)) return;

            SelectedCategories.Add(category);
            SearchText = string.Empty; 
            IsDropdownOpen = false;
        }

        [RelayCommand]
        private void RemoveCategory(Category category)
        {
            if (category != null) SelectedCategories.Remove(category);
        }

        [RelayCommand]
        private async Task CreateNewCategoryAsync()
        {
            string newCatName = SearchText.Trim();
            if (string.IsNullOrEmpty(newCatName)) return;

            var newCategory = await _categoryApiService.CreateCategoryAsync(newCatName);
            if (newCategory != null)
            {
                _allCategories.Add(newCategory);
                SelectedCategories.Add(newCategory);
                SearchText = string.Empty;
                IsDropdownOpen = false;
                OnShowMessage?.Invoke($"Category '{newCatName}' created successfully!");
            }
            else
            {
                OnShowMessage?.Invoke("Failed to create new category.");
            }
        }

        public List<int> GetSelectedCategoryIds()
        {
            return SelectedCategories.Select(c => c.CategoryId).ToList();
        }
    }
}