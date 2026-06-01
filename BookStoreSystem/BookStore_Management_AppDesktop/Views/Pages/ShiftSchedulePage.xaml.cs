using System.Windows.Controls;
using BookStore_Management_AppDesktop.ViewModels;

namespace BookStore_Management_AppDesktop.Views.Pages
{
    /// <summary>
    /// Interaction logic for ShiftSchedulePage.xaml
    /// </summary>
    public partial class ShiftSchedulePage : Page
    {
        public ShiftSchedulePage(ShiftScheduleViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        private void StatusFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is ShiftScheduleViewModel viewModel)
            {
                viewModel.UpdateFiltersCommand.Execute(null);
            }
        }

        private void ShiftFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is ShiftScheduleViewModel viewModel)
            {
                viewModel.UpdateFiltersCommand.Execute(null);
            }
        }
    }
}