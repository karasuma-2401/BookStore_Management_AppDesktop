using BookStore_Management_AppDesktop.Helpers;
using BookStore_Management_AppDesktop.Models;
using BookStore_Management_AppDesktop.Models.DTOs.BookDTOs;
using BookStore_Management_AppDesktop.Services;
using BookStore_Management_AppDesktop.Services.API.BookServices;
using BookStore_Management_AppDesktop.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class SelectBookViewModel : BaseViewModel
    {
        private readonly IBookApiService _apiService;
        private readonly IDialogService _dialogService;
        private readonly IServiceProvider _serviceProvider;
        private readonly DebounceHelper _searchDebouncer = new DebounceHelper();

        private Action<Book, int, decimal> _onBookAddedCallback;

        [ObservableProperty] private string _searchKeyword = string.Empty;
        [ObservableProperty] private ObservableCollection<Book> _searchResults = new ObservableCollection<Book>();
        [ObservableProperty] private int _inputQuantity = 1;
        [ObservableProperty] private decimal _inputPrice = 0;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsBookSelected))]
        private Book? _selectedBook;

        public bool IsBookSelected => SelectedBook != null;

        public SelectBookViewModel(IBookApiService apiService, IDialogService dialogService, IServiceProvider serviceProvider)
        {
            _apiService = apiService;
            _dialogService = dialogService;
            _serviceProvider = serviceProvider;
        }

        public void InitializeCallback(Action<Book, int, decimal> callback)
        {
            _onBookAddedCallback = callback;
        }

        partial void OnSearchKeywordChanged(string value)
        {
            _ = _searchDebouncer.RunAsync(400, async (token) =>
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    Application.Current.Dispatcher.Invoke(() => SearchResults.Clear());
                    return;
                }

                var query = new BookQueryParameters { Keyword = value, PageSize = 15, IncludeOutOfStock = true };
                var results = await _apiService.GetAllBooksAsync(query, token);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    SearchResults.Clear();
                    if (results?.Data != null)
                    {
                        foreach (var book in results.Data) SearchResults.Add(book);
                    }
                });
            });
        }

        [RelayCommand]
        private void AddAndContinue()
        {
            if (ValidateInput())
            {
                _onBookAddedCallback?.Invoke(SelectedBook!, InputQuantity, InputPrice);
                ResetForm();
            }
        }

        [RelayCommand]
        private void AddAndClose(Window currentWindow)
        {
            if (ValidateInput())
            {
                _onBookAddedCallback?.Invoke(SelectedBook!, InputQuantity, InputPrice);
                CloseDialog(currentWindow);
            }
        }

        private bool ValidateInput()
        {
            if (SelectedBook == null) return false;
            if (InputQuantity <= 0 || InputPrice < 0)
            {
                _dialogService.ShowMessage("Quantity must be > 0 and Price cannot be negative.");
                return false;
            }
            return true;
        }

        private void ResetForm()
        {
            SelectedBook = null;
            InputQuantity = 1;
            InputPrice = 0;
            SearchKeyword = string.Empty;
            SearchResults.Clear();
        }

        [RelayCommand]
        private async Task OpenAddNewBook()
        {
            var formVM = _serviceProvider.GetRequiredService<BookFormViewModel>();
            formVM.OnShowMessage = (msg) => _dialogService.ShowMessage(msg);
            await formVM.SetupAddModeAsync();

            Application.Current.Dispatcher.Invoke(() =>
            {
                var addWindow = new BookStore_Management_AppDesktop.Views.Windows.AddBookWindow(formVM);
                addWindow.ShowDialog();
            });
        }

        [RelayCommand]
        private void CloseDialog(Window currentWindow)
        {
            currentWindow?.Close();
        }
    }
}