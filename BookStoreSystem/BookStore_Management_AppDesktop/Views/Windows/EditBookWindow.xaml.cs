using BookStore_Management_AppDesktop.Models;
using System;
using System.Windows;

namespace BookStore_Management_AppDesktop.Views.Windows
{

    public partial class EditBookWindow : Window
    {

        public EditBookWindow(object viewModelContext)
        {
            InitializeComponent();

            this.DataContext = viewModelContext;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}