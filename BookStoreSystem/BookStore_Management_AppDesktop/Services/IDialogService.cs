using BookStore_Management_AppDesktop.Models;

namespace BookStore_Management_AppDesktop.Services
{
    public interface IDialogService
    {
        void ShowMessage(string message);

        bool ShowConfirmation(string message, string confirmText = "Confirm", bool isDanger = false);

        void ShowAddBookWindow();

        void ShowEditBookWindow(Book bookToEdit);
    }
}