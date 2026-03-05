using System.Windows;
using BookStore_Management_AppDesktop.ViewModels;
using Wpf.Ui.Controls;

namespace BookStore_Management_AppDesktop.Views.Windows
{
    public partial class LoginWindow : FluentWindow
    {
        public LoginWindow()
        {
            InitializeComponent();

            DataContext = new LoginViewModel();
        }
    }
}
