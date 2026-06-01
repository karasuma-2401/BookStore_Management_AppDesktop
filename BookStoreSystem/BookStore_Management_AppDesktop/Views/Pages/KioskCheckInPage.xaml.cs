using System.Windows.Controls;
using System.Windows.Input;
using BookStore_Management_AppDesktop.ViewModels;

namespace BookStore_Management_AppDesktop.Views.Pages
{
    /// <summary>
    /// Interaction logic for KioskCheckInPage.xaml
    /// </summary>
    public partial class KioskCheckInPage : Page
    {
        public KioskCheckInPage(KioskCheckInViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        private void EmployeeIdInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var viewModel = this.DataContext as KioskCheckInViewModel;
                if (viewModel != null && viewModel.CheckInCommand.CanExecute(null))
                {
                    viewModel.CheckInCommand.Execute(null);
                }
            }
        }
    }
}
