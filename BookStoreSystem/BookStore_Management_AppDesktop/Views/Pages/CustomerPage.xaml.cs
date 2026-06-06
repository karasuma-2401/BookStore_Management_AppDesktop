using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using BookStore_Management_AppDesktop.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore_Management_AppDesktop.Views.Pages
{
    public partial class CustomerPage : Page
    {
        public CustomerPage()
        {
            InitializeComponent();
            this.DataContext = App.ServiceProvider!.GetRequiredService<CustomerViewModel>();
        }

        private void CustomerDataGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DependencyObject dep = (DependencyObject)e.OriginalSource;

            while ((dep != null) && !(dep is DataGridRow))
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

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.ContextMenu != null)
            {
                element.ContextMenu.PlacementTarget = element;
                element.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                element.ContextMenu.IsOpen = true;
            }
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is CustomerViewModel viewModel)
            {
                if (viewModel.CurrentPage > 1)
                {
                    viewModel.CurrentPage--;
                }
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is CustomerViewModel viewModel)
            {
                if (viewModel.CurrentPage < viewModel.TotalPages)
                {
                    viewModel.CurrentPage++;
                }
            }
        }
    }
}
namespace BookStore_Management_AppDesktop.Converters
{
    public class DebtToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal debt)
            {
                return debt > 0 ? Visibility.Collapsed : Visibility.Visible;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}