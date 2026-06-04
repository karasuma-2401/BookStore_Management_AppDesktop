using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using BookStore_Management_AppDesktop.ViewModels;

namespace BookStore_Management_AppDesktop.Views.Pages
{
    public partial class EmployeePage : Page
    {
        public EmployeePage(EmployeeViewModel viewModel) 
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }


        private void EmployeeDataGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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
                    e.Handled = true; // Prevent DataGrid from re-selecting automatically
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
    }
}