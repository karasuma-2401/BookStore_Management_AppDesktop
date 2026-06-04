using BookStore_Management_AppDesktop.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BookStore_Management_AppDesktop.Views.Pages.InvoiceViews
{
    public partial class InvoicePage : Page
    {
        private readonly InvoiceViewModel _viewModel;

        public InvoicePage(InvoiceViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;

            this.Loaded += InvoicePage_Loaded;
        }

        private async void InvoicePage_Loaded(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                await _viewModel.InitializeDataAsync();
            }
        }

        private void InvoiceDataGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DependencyObject dep = (DependencyObject)e.OriginalSource;
            if (dep == null) return;

            DependencyObject temp = dep;
            while (temp != null)
            {
                if (temp is Button || temp.GetType().Name.Contains("Button") || temp is CheckBox)
                {
                    return;
                }
                temp = VisualTreeHelper.GetParent(temp);
            }

            while (dep != null && !(dep is DataGridRow))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            if (dep is DataGridRow row)
            {
                if (row.IsSelected)
                {
                    row.IsSelected = false;
                    e.Handled = true;
                }
            }
        }

        private void OpenContextMenu_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.ContextMenu != null)
            {
                element.ContextMenu.PlacementTarget = element;
                element.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                element.ContextMenu.IsOpen = true;
            }
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e) => OpenContextMenu_Click(sender, e);
    }
}