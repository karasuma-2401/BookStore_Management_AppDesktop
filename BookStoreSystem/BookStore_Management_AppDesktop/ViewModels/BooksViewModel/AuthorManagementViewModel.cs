using BookStore_Management_AppDesktop.Models; 
using BookStore_Management_AppDesktop.Services;
using BookStore_Management_AppDesktop.Services.API.AuthorServices; 
using BookStore_Management_AppDesktop.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BookStore_Management_AppDesktop.ViewModels.BooksViewModel; 

public partial class AuthorManagementViewModel : BaseViewModel
{
    private readonly IAuthorApiService _authorApiService;
    private readonly IDialogService _dialogService;
    private List<Author> _allAuthorsMaster = new List<Author>();

    [ObservableProperty] private ObservableCollection<Author> _authors = new ObservableCollection<Author>();
    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private bool _isLoading;

    public AuthorManagementViewModel(IAuthorApiService authorApiService, IDialogService dialogService)
    {
        _authorApiService = authorApiService;
        _dialogService = dialogService;
    }

    public override async Task LoadDataAsync()
    {
        await ExecuteLoadAuthorsAsync();
    }

    private async Task ExecuteLoadAuthorsAsync()
    {
        try
        {
            IsLoading = true;
            var result = await _authorApiService.GetAllAuthorsAsync();

            _allAuthorsMaster = result.ToList();
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
            Authors.Clear();
            var filtered = string.IsNullOrWhiteSpace(SearchText)
                ? _allAuthorsMaster
                : _allAuthorsMaster.Where(a => a.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

            foreach (var author in filtered)
            {
                Authors.Add(author);
            }
        });
    }

    [RelayCommand]
    private async Task AddAuthorAsync()
    {
        string? name = _dialogService.ShowInputDialog("Add New Author", "Enter author name:");
        if (string.IsNullOrWhiteSpace(name)) return;

        var newAuthor = await _authorApiService.CreateAuthorAsync(name.Trim());
        if (newAuthor != null)
        {
            _dialogService.ShowMessage("Author added successfully!");
            await ExecuteLoadAuthorsAsync();
        }
        else
        {
            _dialogService.ShowMessage("Failed to create author.");
        }
    }

    [RelayCommand]
    private async Task EditAuthorAsync(Author selectedAuthor)
    {
        if (selectedAuthor == null) return;

        string? newName = _dialogService.ShowInputDialog("Edit Author Name", "Update name:", selectedAuthor.Name);
        if (string.IsNullOrWhiteSpace(newName) || newName.Trim() == selectedAuthor.Name) return;

        bool isSuccess = await _authorApiService.UpdateAuthorAsync(selectedAuthor.AuthorId, newName.Trim());
        if (isSuccess)
        {
            _dialogService.ShowMessage("Author updated successfully!");
            await ExecuteLoadAuthorsAsync();
        }
        else
        {
            _dialogService.ShowMessage("Failed to update author.");
        }
    }

    [RelayCommand]
    private async Task DeleteAuthorAsync(Author selectedAuthor)
    {
        if (selectedAuthor == null) return;

        bool isConfirmed = _dialogService.ShowConfirmation(
            message: $"Are you sure you want to delete author '{selectedAuthor.Name}'?",
            confirmText: "Delete Author",
            isDanger: true);

        if (isConfirmed)
        {
            bool isSuccess = await _authorApiService.DeleteAuthorAsync(selectedAuthor.AuthorId);
            if (isSuccess)
            {
                _dialogService.ShowMessage("Author deleted successfully!");
                await ExecuteLoadAuthorsAsync();
            }
            else
            {
                _dialogService.ShowMessage("Delete failed! This author might be linked to existing books.");
            }
        }
    }
}