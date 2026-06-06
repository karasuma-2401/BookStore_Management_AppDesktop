using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore_Management_AppDesktop.Views.Pages
{
    public partial class RegulationListView : Page
    {
        private bool _isLoaded = false;

        public RegulationListView()
        {
            InitializeComponent();

            DataContext = App.ServiceProvider!
                .GetRequiredService<RegulationViewModel>();

            Loaded += Page_Loaded;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isLoaded) return;

            if (DataContext is RegulationViewModel vm)
            {
                await vm.LoadData();
                _isLoaded = true;
            }
        }
    }
}