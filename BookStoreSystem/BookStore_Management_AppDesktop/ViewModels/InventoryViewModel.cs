using BookStore_Management_AppDesktop.Helpers;
using BookStore_Management_AppDesktop.Messages;
using BookStore_Management_AppDesktop.Models;
using BookStore_Management_AppDesktop.Models.DTOs.BookDTOs; 
using BookStore_Management_AppDesktop.Services;
using BookStore_Management_AppDesktop.Services.API.BookServices;
using BookStore_Management_AppDesktop.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;


namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class InventoryViewModel : BaseViewModel
    {
        private readonly IBookApiService _apiService;
        private readonly CloudinaryService _cloudinaryService;
        private readonly IDialogService _dialogService;

        private readonly DebounceHelper _searchDebouncer = new DebounceHelper();

        [ObservableProperty]
        private ObservableCollection<Book> _books = new ObservableCollection<Book>();

        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty] private int _currentPage = 1;
        [ObservableProperty] private int _pageSize = 10;
        [ObservableProperty] private int _totalItems;
        [ObservableProperty] private int _totalPages = 1;

        [ObservableProperty] private string _sortBy = "price";
        [ObservableProperty] private string _sortOrder = "desc";

        public InventoryViewModel(IBookApiService apiService, CloudinaryService cloudinaryService, IDialogService dialogService)
        {
            _apiService = apiService;
            _cloudinaryService = cloudinaryService;
            _dialogService = dialogService;

            WeakReferenceMessenger.Default.Register<BookChangedMessage>(this, async (recipient, message) =>
            {
                await Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    await LoadDataAsync();
                });
            });
        }

        public override async Task LoadDataAsync()
        {
            var query = new BookQueryParameters
            {
                Keyword = SearchText,
                PageNumber = CurrentPage,
                PageSize = PageSize,
                SortBy = SortBy,
                SortOrder = SortOrder
            };

            var response = await _apiService.GetAllBooksAsync(query);

            Application.Current.Dispatcher.Invoke(() =>
            {
                Books.Clear();
                if (response.Data != null)
                {
                    foreach (var book in response.Data)
                    {
                        Books.Add(book);
                    }
                }

                TotalItems = response.TotalItems;
                TotalPages = response.TotalPages > 0 ? response.TotalPages : 1;

                if (CurrentPage > TotalPages)
                {
                    CurrentPage = TotalPages;
                    _ = LoadDataAsync();
                }
            });
        }

        partial void OnSearchTextChanged(string value)
        {
            _ = _searchDebouncer.RunAsync(997, async (token) =>
            {
                CurrentPage = 1; 
                await LoadDataAsync();
            });
        }

        partial void OnPageSizeChanged(int value)
        {
            CurrentPage = 1;
            _ = LoadDataAsync();
        }

        [RelayCommand]
        private async Task NextPage()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                await LoadDataAsync();
            }
        }

        [RelayCommand]
        private async Task PreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                await LoadDataAsync();
            }
        }

        [RelayCommand]
        private async Task DeleteBookAsync(Book selectedBook)
        {
            if (selectedBook == null) return;

            bool isConfirmed = _dialogService.ShowConfirmation(
                                                        message: "Do you want to delete this book?",
                                                        confirmText: "Delete Book",
                                                        isDanger: true);

            if (isConfirmed)
            {
                bool isSuccess = await _apiService.DeleteBookAsync(selectedBook.BookId);

                if (isSuccess)
                {
                    if (!string.IsNullOrWhiteSpace(selectedBook.ImagePath))
                    {
                        await _cloudinaryService.DeleteImageAsync(selectedBook.ImagePath);
                    }

                    WeakReferenceMessenger.Default.Send(new BookChangedMessage(BookChangedMessage.ChangeAction.Delete, selectedBook));

                    _dialogService.ShowMessage("Book deleted successfully!");
                }
                else
                {
                    _dialogService.ShowMessage("Book delete failed!");
                }
            }
        }


        [RelayCommand]
        private void EditBook(Book selectedBook)
        {
            if (selectedBook == null) return;

            _dialogService.ShowEditBookWindow(selectedBook);
        }
    
    }
}
