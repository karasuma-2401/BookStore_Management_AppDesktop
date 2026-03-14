using BookStore_Management_AppDesktop.Helpers.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace BookStore_Management_AppDesktop.Services.Navigation
{
    /// <summary>
    /// Interface định nghĩa các hành động điều hướng.
    /// </summary>
    public interface INavigationService
    {
        void SetFrame(Frame frame);

        void NavigateTo(PageType pageType);
        void NavigateToMainWindow();
    }
}
