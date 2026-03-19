using BookStore_Management_AppDesktop.Models;
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
using System.Windows.Shapes;

namespace BookStore_Management_AppDesktop.Views.Windows
{
    /// <summary>
    /// Interaction logic for EditBookWindow.xaml
    /// </summary>
    public partial class EditBookWindow : Window
    {
        // Yêu cầu phải truyền vào một đối tượng Book khi mở cửa sổ này
        public EditBookWindow(Book bookToEdit)
        {
            InitializeComponent();

            var viewModelEdit = new EditBookViewModel(bookToEdit);

            viewModelEdit.OnShowMessage = (message) =>
            {
                var msgBox = new CustomMessageBox(message);
                msgBox.ShowDialog();
            };
            viewModelEdit.OnRequestClose = () => this.Close();

            this.DataContext = viewModelEdit;
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
            {
                this.DragMove();
            }
        }
    }
}
