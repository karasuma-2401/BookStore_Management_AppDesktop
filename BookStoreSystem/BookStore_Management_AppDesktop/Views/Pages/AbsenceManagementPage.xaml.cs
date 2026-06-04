using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using BookStore_Management_AppDesktop.ViewModels;

namespace BookStore_Management_AppDesktop.Views.Pages
{
    /// <summary>
    /// Interaction logic for AbsenceManagementPage.xaml
    /// </summary>
    public partial class AbsenceManagementPage : Page
    {
        public AbsenceManagementPage(AbsenceManagementViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        private void AbsenceDataGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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
    }
}
