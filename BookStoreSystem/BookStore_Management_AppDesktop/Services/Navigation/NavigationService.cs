using BookStore_Management_AppDesktop.Helpers.Enums;
using BookStore_Management_AppDesktop.Views.Pages;
using BookStore_Management_AppDesktop.Views.Windows;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace BookStore_Management_AppDesktop.Services.Navigation
{
    public class NavigationService : INavigationService
    {
        private Frame? _mainFrame;

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
                Page? newPage = null;


                switch (pageType)
                {
                    case PageType.Books:
                        newPage = App.ServiceProvider!.GetRequiredService<BookPage>();
                        break;
                    case PageType.Employees:
                        newPage = new EmployeePage();
                        break;
                    case PageType.Inventory:
                        newPage = App.ServiceProvider!.GetRequiredService<InventoryPage>();
                        break;
                    case PageType.Settings:
                        newPage = App.ServiceProvider!.GetRequiredService<SettingsPage>();
                        break;
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



        // Don't touch it please and pull code before push please
        private readonly IServiceProvider _serviceProvider;
        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public void NavigateToMainWindow()
        {
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}