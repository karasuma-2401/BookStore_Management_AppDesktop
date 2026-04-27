using BookStore_Management_AppDesktop.Helpers.Enums;
using BookStore_Management_AppDesktop.Messages;
using BookStore_Management_AppDesktop.Models;
using BookStore_Management_AppDesktop.Models.DTOs;
using BookStore_Management_AppDesktop.Services.API;
using BookStore_Management_AppDesktop.Services.Navigation;
using BookStore_Management_AppDesktop.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class BookViewModel : BaseViewModel
    {
        private readonly IBookApiService _apiService;
        private readonly INavigationService _navigationService;
        private CancellationTokenSource? _searchCts;

        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private string? _selectedSort;

        [ObservableProperty]
        private ObservableCollection<Book> _books = new ObservableCollection<Book>();

        public BookViewModel(IBookApiService apiService, INavigationService navigationService)
        {
            _apiService = apiService;
            _navigationService = navigationService;

            WeakReferenceMessenger.Default.Register<BookChangedMessage>(this, (recipient, message) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    switch (message.Action)
                    {
                        case BookChangedMessage.ChangeAction.Delete:
                            var bookToRemove = Books.FirstOrDefault(b => b.BookId == message.ChangedBook.BookId);
                            if (bookToRemove != null) Books.Remove(bookToRemove);
                            break;

                        case BookChangedMessage.ChangeAction.Add:
                            Books.Insert(0, message.ChangedBook);
                            break;

                        case BookChangedMessage.ChangeAction.Update:
                            var bookToUpdate = Books.FirstOrDefault(b => b.BookId == message.ChangedBook.BookId);
                            if (bookToUpdate != null)
                            {
                                var index = Books.IndexOf(bookToUpdate);
                                Books[index] = message.ChangedBook;
                            }
                            break;
                    }
                });
            });
   
        }

        public override async Task LoadDataAsync()
        {
            await ExecuteSearchAsync();
        }

        partial void OnSearchTextChanged(string value)
        {
            _ = DebounceSearchAsync();
        }

        partial void OnSelectedSortChanged(string? value)
        {
            _ = ExecuteSearchAsync();
        }

        private async Task DebounceSearchAsync()
        {
            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();

            try
            {
                await Task.Delay(997, _searchCts.Token);
                await ExecuteSearchAsync();
            }
            catch (TaskCanceledException) {}
        }

        private async Task ExecuteSearchAsync()
        {
            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();
            var token = _searchCts.Token;

            try
            {
                var query = new BookQueryParameters
                {
                    Keyword = SearchText,
                    SortBy = SelectedSort
                };

                var booksFromApi = await _apiService.GetAllBooksAsync(query, token);


                Application.Current.Dispatcher.Invoke(() =>
                {
                    Books.Clear();
                    foreach (var book in booksFromApi)
                    {
                        Books.Add(book);
                    }
                });
            }
            catch (OperationCanceledException){}
        }

        [RelayCommand]
        private void ViewBookDetail(Book selectedBook)
        {
            if (selectedBook == null) return;

            _navigationService.NavigateTo(PageType.BookDetail, selectedBook.BookId);
        }
    }
}