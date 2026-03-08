using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Generic;
using System.Windows.Controls;
using BookStore_Management_AppDesktop.Views.Pages;
using BookStore_Management_AppDesktop.Helpers.Enums;

namespace BookStore_Management_AppDesktop.Services.Navigation
{
    public class NavigationService : INavigationService
    {
        private Frame _mainFrame;

        // Đây chính là PAGE CACHE: Nơi lưu trữ các trang đã được tạo
        private readonly Dictionary<PageType, Page> _pageCache = new Dictionary<PageType, Page>();

        public void SetFrame(Frame frame)
        {
            _mainFrame = frame;
        }

        public void NavigateTo(PageType pageType)
        {
            if (_mainFrame == null) return;

            if (!_pageCache.ContainsKey(pageType))
            {
                Page newPage = null;

               
                switch (pageType)
                {
                    case PageType.Books:
                        newPage = new BookPage();
                        break;
                    case PageType.Employees:
                        newPage = new EmployeePage();
                        break;
                        // Mở rộng các Page khác...
                }

                
                if (newPage != null)
                {
                    _pageCache.Add(pageType, newPage);
                }
            }

            if (_pageCache.ContainsKey(pageType))
            {
                _mainFrame.Navigate(_pageCache[pageType]);
            }
        }
    }
}
