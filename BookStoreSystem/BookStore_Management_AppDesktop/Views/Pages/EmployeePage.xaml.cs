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
    }
}
