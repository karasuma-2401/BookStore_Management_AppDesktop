using System.Windows;
using System.Windows.Controls;

namespace BookStore_Management_AppDesktop.Views.UserControls
{
    public partial class InventoryTab : UserControl
    {
        public InventoryTab()
        {
            InitializeComponent();
        }
        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}