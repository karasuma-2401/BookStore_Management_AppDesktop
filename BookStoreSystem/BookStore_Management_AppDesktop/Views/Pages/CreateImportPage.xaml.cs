using BookStore_Management_AppDesktop.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BookStore_Management_AppDesktop.Views.Pages
{
    /// <summary>
    /// Interaction logic for CreateImportPage.xaml
    /// </summary>
    public partial class CreateImportPage : Page
    {
        public CreateImportPage(ImportCreateViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
