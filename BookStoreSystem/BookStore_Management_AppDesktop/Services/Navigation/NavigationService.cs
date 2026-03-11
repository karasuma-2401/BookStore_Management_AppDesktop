using System.Windows.Controls;
using BookStore_Management_AppDesktop.Views.Pages;
using BookStore_Management_AppDesktop.Helpers.Enums;
using BookStore_Management_AppDesktop.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore_Management_AppDesktop.Services.Navigation
{
    public class NavigationService : INavigationService
    {

        private Frame _mainFrame;

        private readonly Dictionary<PageType, Page> _pageCache = new Dictionary<PageType, Page>();

        private readonly IServiceProvider _serviceProvider;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

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

        public void NavigateToMainWindow()
        {
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();

            if (mainWindow != null)
            {
                mainWindow.Show();
                System.Windows.Application.Current.Windows[0]?.Close();
            }
        }
    }
}
