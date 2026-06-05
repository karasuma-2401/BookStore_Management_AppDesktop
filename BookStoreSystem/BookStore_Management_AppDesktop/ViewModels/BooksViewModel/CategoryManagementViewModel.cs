using BookStore_Management_AppDesktop.Models;
using BookStore_Management_AppDesktop.Services;
using BookStore_Management_AppDesktop.Services.API.CategoryServices;
using BookStore_Management_AppDesktop.Services.Realtime;
using BookStore_Management_AppDesktop.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BookStore_Management_AppDesktop.ViewModels.BooksViewModel
{
    public partial class CategoryManagementViewModel : BaseViewModel
    {
        private readonly ICategoryApiService _categoryApiService;
        private readonly IBookHubService _hubService;
        private readonly IDialogService _dialogService;

        private List<Category> _allCategoriesMaster = new List<Category>();

        [ObservableProperty] private ObservableCollection<Category> _categories = new ObservableCollection<Category>();
        [ObservableProperty] private string _searchText = string.Empty;
        [ObservableProperty] private bool _isLoading;

        public CategoryManagementViewModel(ICategoryApiService categoryApiService, IBookHubService hubService, IDialogService dialogService)
        {
            _categoryApiService = categoryApiService;
            _hubService = hubService;
            _dialogService = dialogService;

            _hubService.BookUpdated += async (id) => await ExecuteLoadCategoriesAsync();
        }

        public override async Task LoadDataAsync()
        {
            await ExecuteLoadCategoriesAsync();
        }

        private async Task ExecuteLoadCategoriesAsync()
        {
            try
            {
                IsLoading = true;
                var result = await _categoryApiService.GetAllCategoriesAsync();

                _allCategoriesMaster = result.ToList();
                ApplyFilter();
            }
            finally
            {
                IsLoading = false;
            }
        }

        partial void OnSearchTextChanged(string value)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Categories.Clear();
                var filtered = string.IsNullOrWhiteSpace(SearchText)
                    ? _allCategoriesMaster
                    : _allCategoriesMaster.Where(c => c.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

                foreach (var cat in filtered)
                {
                    Categories.Add(cat);
                }
            });
        }

        [RelayCommand]
        private async Task AddCategoryAsync()
        {
            string? name = _dialogService.ShowInputDialog("Add New Category", "Enter category name:");
            if (string.IsNullOrWhiteSpace(name)) return;

            var newCategory = new Category { Name = name.Trim() };

            var result = await _categoryApiService.CreateCategoryAsync(newCategory);

            if (result != null)
            {
                _dialogService.ShowMessage("Category added successfully!");
                await ExecuteLoadCategoriesAsync();
            }
            else
            {
                _dialogService.ShowMessage("Failed to create category.");
            }
        }

        [RelayCommand]
        private async Task EditCategoryAsync(Category selectedCategory)
        {
            if (selectedCategory == null) return;

            string? newName = _dialogService.ShowInputDialog("Edit Category Name", "Update name:", selectedCategory.Name);
            if (string.IsNullOrWhiteSpace(newName) || newName.Trim() == selectedCategory.Name) return;

            bool isSuccess = await _categoryApiService.UpdateCategoryAsync(selectedCategory.CategoryId, newName.Trim());
            if (isSuccess)
            {
                _dialogService.ShowMessage("Category updated successfully!");
                await ExecuteLoadCategoriesAsync();
            }
            else
            {
                _dialogService.ShowMessage("Failed to update category.");
            }
        }

        [RelayCommand]
        private async Task DeleteCategoryAsync(Category selectedCategory)
        {
            if (selectedCategory == null) return;

            bool isConfirmed = _dialogService.ShowConfirmation(
                message: $"Are you sure you want to delete category '{selectedCategory.Name}'?",
                confirmText: "Delete Category",
                isDanger: true);

            if (isConfirmed)
            {
                bool isSuccess = await _categoryApiService.DeleteCategoryAsync(selectedCategory.CategoryId);
                if (isSuccess)
                {
                    _dialogService.ShowMessage("Category deleted successfully!");
                    await ExecuteLoadCategoriesAsync();
                }
                else
                {
                    _dialogService.ShowMessage("Delete failed! This category might be linked to existing books.");
                }
            }
        }
    }
}