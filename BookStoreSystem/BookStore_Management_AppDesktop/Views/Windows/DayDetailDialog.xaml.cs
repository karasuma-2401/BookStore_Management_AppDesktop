using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using MessageBox = BookStore_Management_AppDesktop.Views.Windows.CustomMessageBox;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using BookStore_Management_AppDesktop.Models;
using BookStore_Management_AppDesktop.Models.DTOs.ShiftDTOs;
using BookStore_Management_AppDesktop.Services.API;

namespace BookStore_Management_AppDesktop.Views.Windows
{
    public partial class DayDetailDialog : Window
    {
        private readonly DateTime _selectedDate;
        private readonly IEmployeeShiftApiService _shiftApiService;
        
        public List<Employee> EmployeesList { get; set; }
        public bool IsDataChanged { get; private set; } = false;
        public bool IsPastDate => _selectedDate.Date < DateTime.Today;

        public DayDetailDialog(DateTime selectedDate, IEmployeeShiftApiService shiftApiService, List<Employee> employees)
        {
            InitializeComponent();
            _selectedDate = selectedDate;
            _shiftApiService = shiftApiService;
            EmployeesList = employees ?? new List<Employee>();
            
            this.DataContext = this;
            
            DateTextBlock.Text = _selectedDate.ToString("dddd, dd MMMM yyyy");
            
            this.Loaded += DayDetailDialog_Loaded;
        }

        private async void DayDetailDialog_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadDayDetailsAsync();
        }

        private async Task LoadDayDetailsAsync()
        {
            try
            {
                var result = await _shiftApiService.GetDayDetailAsync(_selectedDate);
                if (result != null)
                {
                    ShiftsItemsControl.ItemsSource = result.Shifts;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadDayDetails Error: {ex.Message}");
                MessageBox.Show($"Failed to load shift details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = IsDataChanged;
            this.Close();
        }

        private async void Assign_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as FrameworkElement;
            var shiftItem = button?.DataContext as ShiftDayItemDto;
            if (shiftItem == null) return;

            // Find parent Border container
            DependencyObject parent = button;
            while (parent != null && !(parent is Border))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            var border = parent as Border;
            if (border == null) return;

            var comboBox = FindVisualChild<ComboBox>(border, "EmployeeComboBox");
            if (comboBox == null || comboBox.SelectedValue == null)
            {
                MessageBox.Show("Please select an employee first.", "Notification", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int employeeId = (int)comboBox.SelectedValue;

            try
            {
                var dto = new ShiftAssignDto
                {
                    EmployeeId = employeeId,
                    ShiftId = shiftItem.ShiftId,
                    WorkDate = _selectedDate
                };

                var (success, message) = await _shiftApiService.AssignShiftAsync(dto);
                if (success)
                {
                    IsDataChanged = true;
                    MessageBox.Show("Shift assigned successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadDayDetailsAsync();
                }
                else
                {
                    MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as FrameworkElement;
            var shiftItem = button?.DataContext as ShiftDayItemDto;
            if (shiftItem == null || !shiftItem.AssignmentId.HasValue) return;

            var result = MessageBox.Show($"Are you sure you want to cancel the shift assignment for {shiftItem.FullName}?", 
                                         "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;

            try
            {
                var success = await _shiftApiService.DeleteAssignmentAsync(shiftItem.AssignmentId.Value);
                if (success)
                {
                    IsDataChanged = true;
                    MessageBox.Show("Shift assignment cancelled successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadDayDetailsAsync();
                }
                else
                {
                    MessageBox.Show("Failed to cancel shift assignment.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private T FindVisualChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T tChild && (child as FrameworkElement)?.Name == childName)
                {
                    return tChild;
                }
                var childOfChild = FindVisualChild<T>(child, childName);
                if (childOfChild != null)
                {
                    return childOfChild;
                }
            }
            return null;
        }
    }
}
