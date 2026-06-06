using System.Windows;
using BookStore_Management_AppDesktop.Models.DTOs.RegulationDTOs;
using BookStore_Management_AppDesktop.Services;
using BookStore_Management_AppDesktop.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore_Management_AppDesktop.Views.Pages
{
    public partial class RegulationEditView : Window
    {
        public RegulationEditView(RegulationResponseDto? item = null)
        {
            InitializeComponent();

            var service = App.ServiceProvider!
                .GetRequiredService<IRegulationApiService>();

            var viewModel = new RegulationEditViewModel(service, item);

     
            viewModel.CloseAction = result =>
            {
                this.DialogResult = result;
                this.Close();
            };

            DataContext = viewModel;
            Title = viewModel.WindowTitle;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}