using BookStore_Management_AppDesktop.Models.DTOs.ImportDTOs;
using BookStore_Management_AppDesktop.ViewModels;
using ClosedXML.Excel;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Input;

namespace BookStore_Management_AppDesktop.Views.Windows
{
    public partial class ImportDetailWindow : Window
    {
        private readonly ImportHistoryUIModel _importData;

        public ImportDetailWindow(ImportHistoryUIModel importData)
        {
            InitializeComponent();

            _importData = importData;
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

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*",
                FileName = $"Import_{_importData.ImportId}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                DefaultExt = ".xlsx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    using var workbook = new XLWorkbook();
                    var worksheet = workbook.Worksheets.Add("Import Details");

                    // Column widths
                    worksheet.Column(1).Width = 12;
                    worksheet.Column(2).Width = 40;
                    worksheet.Column(3).Width = 12;
                    worksheet.Column(4).Width = 14;
                    worksheet.Column(5).Width = 18;
                    worksheet.Column(6).Width = 18;

                    // Title row
                    worksheet.Range("A1:E1").Merge();
                    worksheet.Cell("A1").Value = $"IMPORT DETAIL - ID: {_importData.ImportId}";
                    worksheet.Cell("A1").Style.Font.Bold = true;
                    worksheet.Cell("A1").Style.Font.FontSize = 16;
                    worksheet.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    // Header info
                    worksheet.Cell("A3").Value = "Import Date:";
                    worksheet.Cell("B3").Value = _importData.ImportDate.ToString("dd MMMM yyyy, HH:mm");
                    worksheet.Cell("A4").Value = "Employee:";
                    worksheet.Cell("B4").Value = _importData.EmployeeName;
                    worksheet.Cell("A5").Value = "Total Items:";
                    worksheet.Cell("B5").Value = _importData.TotalItems;
                    worksheet.Cell("A6").Value = "Total Amount:";
                    worksheet.Cell("B6").Value = $"{_importData.TotalAmount:N0} VND";

                    for (int row = 3; row <= 6; row++)
                    {
                        worksheet.Cell($"A{row}").Style.Font.Bold = true;
                    }

                    // Table headers
                    const int headerRow = 8;
                    var headers = new[] { "BOOK ID", "BOOK TITLE", "PUBLISH YEAR", "QUANTITY", "IMPORT PRICE", "TOTAL" };
                    for (int col = 0; col < headers.Length; col++)
                    {
                        var cell = worksheet.Cell(headerRow, col + 1);
                        cell.Value = headers[col];
                        cell.Style.Font.Bold = true;
                        cell.Style.Font.FontColor = XLColor.White;
                        cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#6366F1");
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    }

                    // Table data
                    var details = _importData.OriginalData is ImportResponseDto importData
                        ? importData.Details
                        : new List<ImportDetailResponseDto>();

                    int dataRow = headerRow + 1;
                    int rowIndex = 1;
                    foreach (var detail in details)
                    {
                        var total = detail.Quantity * detail.ImportPrice;

                        worksheet.Cell(dataRow, 1).Value = detail.BookId;
                        worksheet.Cell(dataRow, 2).Value = detail.BookTitle;
                        worksheet.Cell(dataRow, 3).Value = detail.PublishYear;
                        worksheet.Cell(dataRow, 4).Value = detail.Quantity;
                        worksheet.Cell(dataRow, 5).Value = detail.ImportPrice;
                        worksheet.Cell(dataRow, 5).Style.NumberFormat.Format = "#,##0";
                        worksheet.Cell(dataRow, 6).Value = total;
                        worksheet.Cell(dataRow, 6).Style.NumberFormat.Format = "#,##0";

                        // Alignment
                        worksheet.Cell(dataRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        worksheet.Cell(dataRow, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        worksheet.Cell(dataRow, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        worksheet.Cell(dataRow, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        worksheet.Cell(dataRow, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        // Border
                        for (int col = 1; col <= 6; col++)
                        {
                            worksheet.Cell(dataRow, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        }

                        // Alternate row color
                        if (rowIndex % 2 == 0)
                        {
                            for (int col = 1; col <= 6; col++)
                            {
                                worksheet.Cell(dataRow, col).Style.Fill.BackgroundColor = XLColor.FromHtml("#F1F5F9");
                            }
                        }

                        dataRow++;
                        rowIndex++;
                    }

                    // Total row
                    worksheet.Cell(dataRow, 1).Value = "";
                    worksheet.Cell(dataRow, 2).Value = "TOTAL";
                    worksheet.Cell(dataRow, 2).Style.Font.Bold = true;
                    worksheet.Cell(dataRow, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    worksheet.Cell(dataRow, 3).Value = "";
                    worksheet.Cell(dataRow, 4).Value = _importData.TotalItems;
                    worksheet.Cell(dataRow, 4).Style.Font.Bold = true;
                    worksheet.Cell(dataRow, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(dataRow, 5).Value = "";
                    worksheet.Cell(dataRow, 6).Value = _importData.TotalAmount;
                    worksheet.Cell(dataRow, 6).Style.NumberFormat.Format = "#,##0";
                    worksheet.Cell(dataRow, 6).Style.Font.Bold = true;
                    worksheet.Cell(dataRow, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                    for (int col = 1; col <= 6; col++)
                    {
                        worksheet.Cell(dataRow, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        worksheet.Cell(dataRow, col).Style.Fill.BackgroundColor = XLColor.FromHtml("#E0E7FF");
                    }

                    workbook.SaveAs(saveFileDialog.FileName);

                    CustomMessageBox.Show(
                        $"Export successful!\nFile saved to: {saveFileDialog.FileName}",
                        "Success",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    CustomMessageBox.Show(
                        $"Export failed: {ex.Message}",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }
    }
}