using BookStore_Management_AppDesktop.Helpers.Enums;
using BookStore_Management_AppDesktop.Messages;
using BookStore_Management_AppDesktop.Models;
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
            var booksFromApi = await _apiService.GetAllBooksAsync();

            Books = new ObservableCollection<Book>(booksFromApi);
        }

        [RelayCommand]
        private void ViewBookDetail(Book selectedBook)
        {
            if (selectedBook == null) return;

            _navigationService.NavigateTo(PageType.BookDetail, selectedBook.BookId);
        }
    }
}