using BookStore_Management_AppDesktop.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace BookStore_Management_AppDesktop.Views.Windows
{
    public partial class ImportDetailWindow : Window
    {
        public ImportDetailWindow(ImportHistoryUIModel importData)
        {
            InitializeComponent();

            this.DataContext = importData;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
    }
}