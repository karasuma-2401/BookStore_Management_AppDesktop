using System.Windows.Controls;
using BookStore_Management_AppDesktop.ViewModels;
using BookStore_Management_AppDesktop.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore_Management_AppDesktop.Views.Pages
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();

            var viewModel = App.ServiceProvider!.GetRequiredService<SettingsViewModel>();

            viewModel.OnShowMessage = (message) =>
            {
                CustomMessageBox.Show(message);
            };

            this.DataContext = viewModel;

            // Initialize async data
            Loaded += async (s, e) =>
            {
                await viewModel.InitializeAsync();
            };
        }
    }
}
