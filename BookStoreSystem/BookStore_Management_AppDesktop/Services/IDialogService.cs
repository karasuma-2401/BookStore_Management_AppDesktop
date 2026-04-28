using BookStore_Management_AppDesktop.Models;

namespace BookStore_Management_AppDesktop.Services
{
    public interface IDialogService
    {
        void ShowMessage(string message);

        bool ShowDeleteConfirmation();

        void ShowAddBookWindow();

        void ShowEditBookWindow(Book bookToEdit);
    }
}