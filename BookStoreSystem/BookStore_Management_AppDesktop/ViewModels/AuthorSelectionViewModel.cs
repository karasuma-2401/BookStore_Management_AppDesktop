using BookStore_Management_AppDesktop.Models;
using BookStore_Management_AppDesktop.Models.DTOs;
using BookStore_Management_AppDesktop.Services.API;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class AuthorSelectionViewModel : ObservableObject
    {
        private readonly IAuthorApiService _authorApiService;

        public Action<string>? OnShowMessage { get; set; }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddAuthorCommand))]
        private bool _isLoading;

        [ObservableProperty] private bool _isShowAddButton;

        private List<Author> _allAuthors = new();

        [ObservableProperty]
        private ObservableCollection<Author> _displayAuthors = new();

        private Author? _selectedAuthor;
        public Author? SelectedAuthor
        {
            get => _selectedAuthor;
            set
            {
                if (SetProperty(ref _selectedAuthor, value))
                {
                    if (value != null && !string.Equals(SearchAuthorText, value.Name, StringComparison.Ordinal))
                    {
                        SearchAuthorText = value.Name;
                    }
                }
            }
        }

        private string _searchAuthorText = string.Empty;
        public string SearchAuthorText
        {
            get => _searchAuthorText;
            set
            {
                var text = value?.Trim() ?? string.Empty;
                if (SetProperty(ref _searchAuthorText, text))
                {
                    if (_selectedAuthor != null && !string.Equals(_selectedAuthor.Name, text, StringComparison.OrdinalIgnoreCase))
                    {
                        SelectedAuthor = null;
                    }

                    UpdateDisplayAuthors(text);

                    IsShowAddButton = !string.IsNullOrWhiteSpace(text) &&
                                      !_allAuthors.Any(a => string.Equals(a.Name, text, StringComparison.OrdinalIgnoreCase));
                }
            }
        }

        public AuthorSelectionViewModel(IAuthorApiService authorApiService)
        {
            _authorApiService = authorApiService;
        }

        public async Task InitializeAsync(int? initialAuthorId = null)
        {
            try
            {
                var authors = await _authorApiService.GetAllAuthorsAsync();

                _allAuthors = authors.ToList();

                UpdateDisplayAuthors(SearchAuthorText);

                if (initialAuthorId.HasValue)
                {
                    SelectedAuthor = _allAuthors.FirstOrDefault(a => a.AuthorId == initialAuthorId.Value);

                    if (SelectedAuthor != null && !DisplayAuthors.Contains(SelectedAuthor))
                    {
                        DisplayAuthors.Insert(0, SelectedAuthor);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                OnShowMessage?.Invoke("Failed to load authors.");
            }
        }

        private void UpdateDisplayAuthors(string searchText)
        {
            DisplayAuthors.Clear();
            IEnumerable<Author> filteredList;

            if (string.IsNullOrWhiteSpace(searchText))
            {
                filteredList = _allAuthors.Take(5);
            }
            else
            {
                filteredList = _allAuthors
                    .Where(a => a.Name?.Contains(searchText, StringComparison.OrdinalIgnoreCase) == true)
                    .Take(5);
            }

            foreach (var author in filteredList)
            {
                DisplayAuthors.Add(author);
            }
        }

        private bool CanExecuteAction() => !IsLoading;

        [RelayCommand(CanExecute = nameof(CanExecuteAction))]
        private async Task AddAuthorAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchAuthorText)) return;

            try
            {
                IsLoading = true;
                var created = await _authorApiService.CreateAuthorAsync(new AuthorCreateDto { Name = SearchAuthorText.Trim() });

                if (created == null)
                {
                    OnShowMessage?.Invoke("Add author failed.");
                    return;
                }

                _allAuthors.Add(created);

                SelectedAuthor = created;

                OnShowMessage?.Invoke($"Added '{created.Name}'");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                OnShowMessage?.Invoke("Add author failed.");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}