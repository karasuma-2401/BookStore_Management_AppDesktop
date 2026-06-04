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
            DependencyObject originalSource = (DependencyObject)e.OriginalSource;
            if (originalSource == null) return;

            if (IsVisualAncestorOfButton(originalSource)) return;

            DependencyObject dep = originalSource;
            while (dep != null && !(dep is DataGridRow) && !(dep is DataGrid))
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

        private bool IsVisualAncestorOfButton(DependencyObject? node)
        {
            while (node != null)
            {
                if (node is Button || node.GetType().Name.Contains("Button"))
                {
                    return true;
                }
                node = VisualTreeHelper.GetParent(node);
            }
            return false;
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