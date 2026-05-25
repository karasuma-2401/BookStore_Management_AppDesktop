using BookStore_Management_AppDesktop.Models;
using BookStore_Management_AppDesktop.Models.DTOs.CustomerDTOs;
using BookStore_Management_AppDesktop.Views.Windows;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

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
            var msgBox = new CustomMessageBox(message);
            msgBox.ShowDialog();
        }

        public bool ShowConfirmation(string message, string confirmText = "Confirm", bool isDanger = false)
        {
            var window = new ConfirmWindow(message, confirmText, isDanger);
            window.ShowDialog();
            return window.DialogResult == true;
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

        public CustomerResponseDto? ShowAddCustomerWindow() // Trả về ResponseDto thay vì CreateDto
        {
            var window = _serviceProvider.GetRequiredService<AddCustomerWindow>();

            if (window.ShowDialog() == true)
            {
                // Bạn cần thêm thuộc tính CustomerResult vào AddCustomerWindow
                return window.CustomerResult;
            }
            return null;
        }
    }
}
