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
using BookStore_Management_AppDesktop.ViewModels;

namespace BookStore_Management_AppDesktop.Views.Pages
{
    /// <summary>
    /// Interaction logic for EmployeePage.xaml
    /// </summary>
    public partial class EmployeePage : Page
    {
        public EmployeePage()
        {
            InitializeComponent();
            // Gán ViewModel để UI có thể Binding dữ liệu từ danh sách Employees
            this.DataContext = new EmployeeViewModel();
        }

        /// <summary>
        /// Logic xử lý: Click 1 lần chọn dòng (màu đỏ), click lần nữa vào dòng đó thì hủy chọn.
        /// </summary>
        private void EmployeeDataGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Tìm xem element bị click có nằm trong một DataGridRow hay không
            DependencyObject dep = (DependencyObject)e.OriginalSource;

            while ((dep != null) && !(dep is DataGridRow))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            if (dep is DataGridRow row)
            {
                // Nếu dòng này đã được chọn trước đó, chúng ta sẽ hủy chọn nó
                if (row.IsSelected)
                {
                    row.IsSelected = false;

                    // Ngừng sự kiện tại đây để DataGrid không tự động chọn lại dòng này
                    e.Handled = true;
                }
                // Nếu chưa chọn, DataGrid sẽ tự thực hiện logic chọn mặc định (Trigger sẽ đổi màu chữ sang Đỏ)
            }
        }
    }
}
