using BookStore_Management_AppDesktop.Helpers;
using BookStore_Management_AppDesktop.Messages;
using BookStore_Management_AppDesktop.Models;
using BookStore_Management_AppDesktop.Models.DTOs;
using BookStore_Management_AppDesktop.Services; // Dùng IDialogService
using BookStore_Management_AppDesktop.Services.API;
using BookStore_Management_AppDesktop.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging; // Dùng Messenger
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class ImportCreateViewModel : BaseViewModel
    {
        private readonly IBookApiService _apiService;
        private readonly IDialogService _dialogService; 
        private readonly DebounceHelper _searchDebouncer = new DebounceHelper();
        private readonly IImportApiService _importApiService;

        [ObservableProperty] private ObservableCollection<ImportCartItem> _draftList = new ObservableCollection<ImportCartItem>();
        [ObservableProperty] private string _searchText = string.Empty;
        [ObservableProperty] private ObservableCollection<Book> _searchResults = new ObservableCollection<Book>();
        [ObservableProperty] private Book? _selectedSearchResult;
        [ObservableProperty] private int _tempImportQuantity = 1;
        [ObservableProperty] private decimal _tempImportPrice = 0;
        [ObservableProperty] private decimal _totalDraftAmount = 0;

        public ImportCreateViewModel(IBookApiService apiService, IDialogService dialogService)
        {
            _apiService = apiService;
            _dialogService = dialogService;

            WeakReferenceMessenger.Default.Register<BookChangedMessage>(this, (recipient, message) =>
            {
                if (message.Action == BookChangedMessage.ChangeAction.Add)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var newBook = message.ChangedBook;

                        var newItem = new ImportCartItem
                        {
                            BookId = newBook.BookId,
                            Title = newBook.Title ?? "New Book",
                            AuthorName = newBook.AuthorName ?? string.Empty,
                            CurrentQuantity = newBook.Quantity,
                            ImportQuantity = 1, 
                            ImportPrice = 0    
                        };

                        newItem.PropertyChanged += DraftItem_PropertyChanged;
                        DraftList.Add(newItem);
                        RecalculateTotalAmount();
                    });
                }
            });
        }

        partial void OnSearchTextChanged(string value)
        {
            _ = _searchDebouncer.RunAsync(400, async (token) =>
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    Application.Current.Dispatcher.Invoke(() => SearchResults.Clear());
                    return;
                }

                var query = new BookQueryParameters { Keyword = SearchText, PageSize = 10 };
                var results = await _apiService.GetAllBooksAsync(query, token);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    SearchResults.Clear();
                    foreach (var book in results) SearchResults.Add(book);
                });
            });
        }

        [RelayCommand]
        private void AddToDraft()
        {
            if (SelectedSearchResult == null)
            {
                _dialogService.ShowMessage("Please choose a book from the suggested list!");
                return;
            }

            if (TempImportQuantity <= 0 || TempImportPrice < 0)
            {
                _dialogService.ShowMessage("Quantity must be > 0 and Input price cannot be negative!");
                return;
            }

            var existingItem = DraftList.FirstOrDefault(x => x.BookId == SelectedSearchResult.BookId);

            if (existingItem != null)
            {
                existingItem.ImportQuantity += TempImportQuantity;
                existingItem.ImportPrice = TempImportPrice;
                RecalculateTotalAmount();
            }
            else
            {
                var newItem = new ImportCartItem
                {
                    BookId = SelectedSearchResult.BookId,
                    Title = SelectedSearchResult.Title ?? string.Empty,
                    AuthorName = SelectedSearchResult.AuthorName ?? string.Empty,
                    CurrentQuantity = SelectedSearchResult.Quantity,
                    ImportQuantity = TempImportQuantity,
                    ImportPrice = TempImportPrice
                };
                newItem.PropertyChanged += DraftItem_PropertyChanged;
                DraftList.Add(newItem);
                RecalculateTotalAmount();
            }

            SelectedSearchResult = null;
            SearchText = string.Empty;
            TempImportQuantity = 1;
            TempImportPrice = 0;
            SearchResults.Clear();
        }

        private void RecalculateTotalAmount() => TotalDraftAmount = DraftList.Sum(item => item.TotalLinePrice);

        private void DraftItem_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ImportCartItem.ImportQuantity) || e.PropertyName == nameof(ImportCartItem.ImportPrice))
            {
                RecalculateTotalAmount();
            }
        }

        [RelayCommand]
        private void OpenCreateNewBookDialog()
        {
            _dialogService.ShowAddBookWindow();
        }

        [RelayCommand]
        private void RemoveFromDraft(ImportCartItem itemToRemove)
        {
            if (itemToRemove != null)
            {
                itemToRemove.PropertyChanged -= DraftItem_PropertyChanged;
                DraftList.Remove(itemToRemove);
                RecalculateTotalAmount();
            }
        }

        [RelayCommand]
        private void ClearDraft()
        {
            bool isConfirmed = _dialogService.ShowDeleteConfirmation();
            if (isConfirmed)
            {
                ClearDraftSafe();
            }
        }

        public void ClearDraftSafe()
        {
            foreach (var item in DraftList) item.PropertyChanged -= DraftItem_PropertyChanged;
            DraftList.Clear();
            RecalculateTotalAmount();
        }

        [RelayCommand]
        private async Task ConfirmImport()
        {
            if (!DraftList.Any())
            {
                _dialogService.ShowMessage("Your draft list is empty!");
                return;
            }

            bool isConfirmed = _dialogService.ShowDeleteConfirmation();

            if (isConfirmed)
            {
                var importDto = new ImportCreateDTO
                {
                    UserId = 1, 
                    Details = DraftList.Select(item => new ImportDetailCreateDTO
                    {
                        BookId = item.BookId,
                        Quantity = item.ImportQuantity,
                        ImportPrice = item.ImportPrice
                    }).ToList()
                };

                try
                {
                    bool isSuccess = await _importApiService.CreateImportAsync(importDto);

                    if (isSuccess)
                    {
                        _dialogService.ShowMessage("Import successful! The inventory has been updated.");
                        ClearDraftSafe(); 
                    }
                    else
                    {
                        _dialogService.ShowMessage("Import failed. Please check the server connection.");
                    }
                }
                catch (System.Exception ex)
                {
                    _dialogService.ShowMessage($"An error occurred: {ex.Message}");
                }
            }
        }
    }
}