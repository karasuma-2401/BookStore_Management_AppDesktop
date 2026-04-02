using CommunityToolkit.Mvvm.Input;
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
    /// Interaction logic for AvatarEditView.xaml
    /// </summary>
    public partial class AvatarEditView : Window
    {
        public AvatarEditView(object dataContext)
        {
            InitializeComponent();
            this.DataContext = dataContext; // Nhận ViewModel từ Page truyền sang
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
    }
}
