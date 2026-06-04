using BookStore_Management_AppDesktop.Models;
using BookStore_Management_AppDesktop.Models.DTOs.CustomerDTOs;
using BookStore_Management_AppDesktop.Views.Windows;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BookStore_Management_AppDesktop.Services
{
    public class DialogService : IDialogService
    {
        private readonly IServiceProvider _serviceProvider;

        public DialogService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void ShowMessage(string message)
        {
            CustomMessageBox.Show(message, "Notification", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }

        public bool ShowConfirmation(string message, string confirmText = "Confirm", bool isDanger = false)
        {
            var icon = isDanger ? System.Windows.MessageBoxImage.Warning : System.Windows.MessageBoxImage.Question;
            var result = CustomMessageBox.Show(message, "Confirmation", System.Windows.MessageBoxButton.YesNo, icon);
            return result == System.Windows.MessageBoxResult.Yes;
        }

        public void ShowAddBookWindow()
        {
            var addWindow = _serviceProvider.GetRequiredService<AddBookWindow>();
            addWindow.ShowDialog();
        }

        public void ShowEditBookWindow(Book bookToEdit)
        {
            var editWindow = new EditBookWindow(bookToEdit);
            editWindow.ShowDialog();
        }

        public CustomerResponseDto? ShowAddCustomerWindow()
        {
            var window = _serviceProvider.GetRequiredService<AddCustomerWindow>();
            if (window.ShowDialog() == true)
            {
                return window.CustomerResult;
            }
            return null;
        }

        public string? ShowInputDialog(string title, string message, string defaultText = "")
        {
            var window = new InputDialogWindow(title, message, defaultText)
            {
                Owner = System.Windows.Application.Current.MainWindow
            };

            if (window.ShowDialog() == true)
            {
                return window.InputText;
            }
            return null;
        }

        public void ShowAuthorManagementWindow()
        {
            var window = _serviceProvider.GetRequiredService<AuthorManagementWindow>();
            window.Owner = System.Windows.Application.Current.MainWindow;
            window.ShowDialog();
        }

        public void ShowCategoryManagementWindow()
        {
            var categoryWindow = _serviceProvider.GetRequiredService<CategoryManagementWindow>();
            categoryWindow.Owner = System.Windows.Application.Current.MainWindow;
            categoryWindow.ShowDialog();
        }

        public async Task ShowErrorAsync(string title, string message)
        {
            CustomMessageBox.Show(message, title, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            await Task.CompletedTask;
        }

        public async Task ShowSuccessAsync(string title, string message)
        {
            CustomMessageBox.Show(message, title, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            await Task.CompletedTask;
        }

        public async Task<bool> ShowConfirmationAsync(string title, string message)
        {
            var result = CustomMessageBox.Show(message, title, System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);
            return await Task.FromResult(result == System.Windows.MessageBoxResult.Yes);
        }
    }
}