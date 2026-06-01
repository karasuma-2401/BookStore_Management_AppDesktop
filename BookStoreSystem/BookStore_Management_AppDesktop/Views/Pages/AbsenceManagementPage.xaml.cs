using System.Windows.Controls;
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
    }
}
