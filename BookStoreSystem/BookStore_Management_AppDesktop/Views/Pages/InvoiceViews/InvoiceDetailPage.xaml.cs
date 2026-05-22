using BookStore_Management_AppDesktop.ViewModels;
using BookStore_Management_AppDesktop.Services.Navigation;
using BookStore_Management_AppDesktop.ViewModels;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BookStore_Management_AppDesktop.Views.Pages.InvoiceViews
{
    public partial class InvoiceDetailPage : Page, INavigatable
    {
        public InvoiceDetailPage(InvoiceDetailViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        public async void OnNavigatedTo(object? parameter)
        {
            if (DataContext is InvoiceDetailViewModel vm && parameter is int invoiceId)
            {
                await vm.LoadInvoiceAsync(invoiceId);
            }
        }
    }
}