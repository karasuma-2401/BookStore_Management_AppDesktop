using BookStore_Management_AppDesktop.Models;
using BookStore_Management_AppDesktop.Models.DTOs.CustomerDTOs;
using System.Threading.Tasks;

namespace BookStore_Management_AppDesktop.Services
{
    public interface IDialogService
    {
        void ShowMessage(string message);
        bool ShowConfirmation(string message, string confirmText = "Confirm", bool isDanger = false);
        void ShowAddBookWindow();
        void ShowEditBookWindow(Book bookToEdit);
        CustomerResponseDto? ShowAddCustomerWindow();

        string? ShowInputDialog(string title, string message, string defaultText = "");
        void ShowAuthorManagementWindow();
        void ShowCategoryManagementWindow();

        Task ShowErrorAsync(string title, string message);
        Task ShowSuccessAsync(string title, string message);
        Task<bool> ShowConfirmationAsync(string title, string message);
    }
}