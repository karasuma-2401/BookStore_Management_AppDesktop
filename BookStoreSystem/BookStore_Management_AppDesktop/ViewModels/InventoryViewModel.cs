using BookStore_Management_AppDesktop.Helpers;
using BookStore_Management_AppDesktop.Models;
using BookStore_Management_AppDesktop.Services;
using BookStore_Management_AppDesktop.Services.API;
using BookStore_Management_AppDesktop.Views.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BookStore_Management_AppDesktop.ViewModels.Base; 
using System.Collections.ObjectModel;
using System.Windows;
using static System.Reflection.Metadata.BlobBuilder;
using CommunityToolkit.Mvvm.Messaging;
using BookStore_Management_AppDesktop.Messages;
using System.Linq;


namespace BookStore_Management_AppDesktop.ViewModels
{
    public partial class InventoryViewModel : BaseViewModel
    {
        private readonly IBookApiService _apiService;
        private readonly CloudinaryService _cloudinaryService;

        [ObservableProperty]
        private ObservableCollection<Book> _books = new ObservableCollection<Book>();

        [ObservableProperty]
        private string _searchText = string.Empty;

        public InventoryViewModel(IBookApiService apiService, CloudinaryService cloudinaryService)
        {
            _apiService = apiService;
            _cloudinaryService = cloudinaryService;

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


        public Action<string>? OnShowMessage { get; set; }
        public Func<string, string, Task<bool>>? OnRequestConfirm { get; set; }

        [RelayCommand]
        private async Task DeleteBookAsync(Book selectedBook)
        {
            if (selectedBook == null) return;

            bool isConfirmed = false;
            if (OnRequestConfirm != null)
            {
                isConfirmed = await OnRequestConfirm.Invoke("Confirm Deletion", $"Are you sure you want to delete the book '{selectedBook.Title}'?");
            }

            if (isConfirmed)
            {

                bool isSuccess = await _apiService.DeleteBookAsync(selectedBook.BookId);

                if (isSuccess)
                {
                    if (!string.IsNullOrWhiteSpace(selectedBook.ImagePath))
                    {
                        await _cloudinaryService.DeleteImageAsync(selectedBook.ImagePath);
                    }

                    Books.Remove(selectedBook);

                    WeakReferenceMessenger.Default.Send(new BookChangedMessage(BookChangedMessage.ChangeAction.Delete, selectedBook));

                    OnShowMessage?.Invoke("Book deleted successfully!");
                }
                else
                {
                    OnShowMessage?.Invoke("Book delete failed!");
                }
            }
        }


        [RelayCommand]
        private void EditBook(Book selectedBook)
        {
            if (selectedBook == null) return;

            var editWindow = new EditBookWindow(selectedBook);
            editWindow.ShowDialog(); 

        }
    }
}
