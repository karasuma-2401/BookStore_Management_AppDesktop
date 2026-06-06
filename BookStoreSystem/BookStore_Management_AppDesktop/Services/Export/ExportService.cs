using BookStore_Management_AppDesktop.Models.DTOs.ReportDTOs;
using BookStore_Management_AppDesktop.Models.DTOs.ShiftDTOs;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MessageBox = BookStore_Management_AppDesktop.Views.Windows.CustomMessageBox;

namespace BookStore_Management_AppDesktop.Services.Export
{
    public class ExportService : IExportService
    {
        public async Task<bool> ExportReportToExcelAsync(
            int month,
            int year,
            decimal totalRevenue,
            decimal totalImportCost,
            decimal profit,
            int totalBooksSold,
            ObservableCollection<TopBookDto> topBooks)
        {
            try
            {
                var fileName = $"Report_{month}_{year}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                var downloadsPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                downloadsPath = Path.Combine(downloadsPath, "Downloads");

                if (!Directory.Exists(downloadsPath))
                {
                    Directory.CreateDirectory(downloadsPath);
                }

                var filePath = Path.Combine(downloadsPath, fileName);

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Report");

                    // Set column widths
                    worksheet.Column(1).Width = 30;
                    worksheet.Column(2).Width = 20;

                    // Title
                    var titleCell = worksheet.Cell("A1");
                    titleCell.Value = $"Financial Report - {GetMonthName(month)} {year}";
                    titleCell.Style.Font.Bold = true;
                    titleCell.Style.Font.FontSize = 16;
                    worksheet.Range("A1:B1").Merge();

                    // Summary Section
                    var currentRow = 3;

                    // Summary Header
                    var summaryHeaderCell = worksheet.Cell(currentRow, 1);
                    summaryHeaderCell.Value = "SUMMARY";
                    summaryHeaderCell.Style.Font.Bold = true;
                    summaryHeaderCell.Style.Font.FontSize = 12;
                    worksheet.Range(currentRow, 1, currentRow, 2).Merge();

                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = "Total Revenue";
                    worksheet.Cell(currentRow, 2).Value = totalRevenue;
                    worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "#,##0.00";

                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = "Total Import Cost";
                    worksheet.Cell(currentRow, 2).Value = totalImportCost;
                    worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "#,##0.00";

                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = "Net Profit";
                    worksheet.Cell(currentRow, 2).Value = profit;
                    worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "#,##0.00";
                    worksheet.Cell(currentRow, 2).Style.Font.Bold = true;

                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = "Total Books Sold";
                    worksheet.Cell(currentRow, 2).Value = totalBooksSold;

                    // Top Books Section
                    currentRow += 2;
                    var topBooksHeaderCell = worksheet.Cell(currentRow, 1);
                    topBooksHeaderCell.Value = "TOP SELLING BOOKS";
                    topBooksHeaderCell.Style.Font.Bold = true;
                    topBooksHeaderCell.Style.Font.FontSize = 12;
                    worksheet.Range(currentRow, 1, currentRow, 3).Merge();

                    currentRow++;
                    // Headers for top books table
                    worksheet.Cell(currentRow, 1).Value = "Book Title";
                    worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 1).Style.Fill.BackgroundColor = XLColor.LightGray;

                    worksheet.Cell(currentRow, 2).Value = "Total Sold";
                    worksheet.Cell(currentRow, 2).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 2).Style.Fill.BackgroundColor = XLColor.LightGray;

                    worksheet.Cell(currentRow, 3).Value = "Revenue Generated";
                    worksheet.Cell(currentRow, 3).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 3).Style.Fill.BackgroundColor = XLColor.LightGray;

                    currentRow++;

                    // Add top books data
                    foreach (var book in topBooks)
                    {
                        worksheet.Cell(currentRow, 1).Value = book.Title;
                        worksheet.Cell(currentRow, 2).Value = book.TotalSold;
                        worksheet.Cell(currentRow, 3).Value = book.RevenueGenerated;
                        worksheet.Cell(currentRow, 3).Style.NumberFormat.Format = "#,##0.00";
                        currentRow++;
                    }

                    // Set column widths for top books
                    worksheet.Column(3).Width = 20;

                    workbook.SaveAs(filePath);
                }

                MessageBox.Show($"Report exported successfully to:\n{filePath}", "Export Success", MessageBoxButton.OK, MessageBoxImage.Information);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting report: {ex.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public async Task<bool> ExportScheduleToExcelAsync(
            int month,
            int year,
            IEnumerable<EmployeeShiftResponseDto> shifts)
        {
            try
            {
                var fileName = $"ShiftSchedule_{GetMonthName(month)}_{year}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                var downloadsPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                downloadsPath = Path.Combine(downloadsPath, "Downloads");

                if (!Directory.Exists(downloadsPath))
                {
                    Directory.CreateDirectory(downloadsPath);
                }

                var filePath = Path.Combine(downloadsPath, fileName);

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Schedule");

                    // Title block
                    var titleCell = worksheet.Cell("A1");
                    titleCell.Value = $"SHIFT SCHEDULE - {GetMonthName(month).ToUpper()} {year}";
                    titleCell.Style.Font.Bold = true;
                    titleCell.Style.Font.FontSize = 16;
                    titleCell.Style.Font.FontColor = XLColor.White;
                    titleCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#0F172A");
                    titleCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Range("A1:F1").Merge();

                    // Metadata
                    worksheet.Cell(3, 1).Value = "Exported On:";
                    worksheet.Cell(3, 1).Style.Font.Bold = true;
                    worksheet.Cell(3, 2).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    int total = shifts.Count();
                    int present = shifts.Count(s => s.Status.Equals("Present", StringComparison.OrdinalIgnoreCase));
                    int late = shifts.Count(s => s.Status.Equals("Late", StringComparison.OrdinalIgnoreCase));
                    int absent = shifts.Count(s => s.Status.Equals("Absent", StringComparison.OrdinalIgnoreCase));
                    int compensated = shifts.Count(s => s.Status.Equals("Compensated", StringComparison.OrdinalIgnoreCase));
                    int scheduled = shifts.Count(s => s.Status.Equals("Scheduled", StringComparison.OrdinalIgnoreCase));

                    worksheet.Cell(4, 1).Value = "Total Shifts:";
                    worksheet.Cell(4, 1).Style.Font.Bold = true;
                    worksheet.Cell(4, 2).Value = total;

                    worksheet.Cell(5, 1).Value = "Breakdown:";
                    worksheet.Cell(5, 1).Style.Font.Bold = true;
                    worksheet.Cell(5, 2).Value = $"Present: {present} | Late: {late} | Absent: {absent} | Compensated: {compensated} | Scheduled: {scheduled}";
                    worksheet.Range("B5:F5").Merge();

                    // Table headers
                    var headers = new[] { "Date", "Day of Week", "Employee Name", "Shift", "Time", "Status" };
                    for (int col = 1; col <= headers.Length; col++)
                    {
                        var cell = worksheet.Cell(7, col);
                        cell.Value = headers[col - 1];
                        cell.Style.Font.Bold = true;
                        cell.Style.Font.FontColor = XLColor.White;
                        cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#1E293B");
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    }

                    // Table data
                    int currentRow = 8;
                    foreach (var s in shifts.OrderBy(x => x.WorkDate))
                    {
                        var dateCell = worksheet.Cell(currentRow, 1);
                        dateCell.Value = s.WorkDate;
                        dateCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        string dayName = "Unknown";
                        if (DateTime.TryParse(s.WorkDate, out var dt))
                        {
                            dayName = dt.ToString("dddd");
                        }
                        var dayCell = worksheet.Cell(currentRow, 2);
                        dayCell.Value = dayName;
                        dayCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        worksheet.Cell(currentRow, 3).Value = s.FullName;

                        var shiftCell = worksheet.Cell(currentRow, 4);
                        shiftCell.Value = s.ShiftName;
                        shiftCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        var timeCell = worksheet.Cell(currentRow, 5);
                        timeCell.Value = s.WorkTime;
                        timeCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        var statusCell = worksheet.Cell(currentRow, 6);
                        statusCell.Value = s.Status;
                        statusCell.Style.Font.Bold = true;
                        statusCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        // Apply status coloring
                        var statusColor = s.Status.ToLower() switch
                        {
                            "present" => XLColor.FromHtml("#10B981"),
                            "late" => XLColor.FromHtml("#F97316"),
                            "absent" => XLColor.FromHtml("#EF4444"),
                            "compensated" => XLColor.FromHtml("#F59E0B"),
                            "scheduled" => XLColor.FromHtml("#0EA5E9"),
                            _ => XLColor.Gray
                        };
                        statusCell.Style.Font.FontColor = statusColor;

                        // Add borders
                        for (int col = 1; col <= 6; col++)
                        {
                            worksheet.Cell(currentRow, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            if (currentRow % 2 == 0)
                            {
                                worksheet.Cell(currentRow, col).Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FAFC");
                            }
                        }

                        currentRow++;
                    }

                    // Auto fit column widths
                    worksheet.Columns().AdjustToContents();

                    workbook.SaveAs(filePath);
                }

                MessageBox.Show($"Schedule exported successfully to:\n{filePath}", "Export Success", MessageBoxButton.OK, MessageBoxImage.Information);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting schedule: {ex.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public async Task<bool> ExportInventoryToExcelAsync(IEnumerable<Models.Book> books)
        {
            try
            {
                var fileName = $"Inventory_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                var downloadsPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                downloadsPath = Path.Combine(downloadsPath, "Downloads");

                if (!Directory.Exists(downloadsPath))
                {
                    Directory.CreateDirectory(downloadsPath);
                }

                var filePath = Path.Combine(downloadsPath, fileName);

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Inventory");

                    // Title
                    var titleCell = worksheet.Cell("A1");
                    titleCell.Value = "INVENTORY REPORT";
                    titleCell.Style.Font.Bold = true;
                    titleCell.Style.Font.FontSize = 16;
                    titleCell.Style.Font.FontColor = XLColor.White;
                    titleCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#0F172A");
                    worksheet.Range("A1:F1").Merge();

                    // Metadata
                    worksheet.Cell(2, 1).Value = $"Exported: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                    worksheet.Cell(2, 1).Style.Font.Italic = true;
                    worksheet.Cell(2, 1).Style.Font.FontSize = 11;

                    // Summary
                    var bookList = books.ToList();
                    int totalBooks = bookList.Count;
                    int lowStockBooks = bookList.Count(b => b.Quantity > 0 && b.Quantity <= 5);
                    long totalQuantity = bookList.Sum(b => b.Quantity);
                    decimal totalValue = bookList.Sum(b => (long)b.Quantity * b.Price);

                    worksheet.Cell(4, 1).Value = "SUMMARY";
                    worksheet.Cell(4, 1).Style.Font.Bold = true;
                    worksheet.Cell(4, 1).Style.Font.FontSize = 12;

                    worksheet.Cell(5, 1).Value = "Total Books (Types):";
                    worksheet.Cell(5, 2).Value = totalBooks;
                    worksheet.Cell(5, 2).Style.Font.Bold = true;

                    worksheet.Cell(6, 1).Value = "Total Stock Quantity:";
                    worksheet.Cell(6, 2).Value = totalQuantity;
                    worksheet.Cell(6, 2).Style.Font.Bold = true;

                    worksheet.Cell(7, 1).Value = "Low Stock Items (1-5):";
                    worksheet.Cell(7, 2).Value = lowStockBooks;
                    worksheet.Cell(7, 2).Style.Font.Bold = true;
                    worksheet.Cell(7, 2).Style.Font.FontColor = XLColor.FromHtml("#EF4444");

                    worksheet.Cell(8, 1).Value = "Total Inventory Value:";
                    worksheet.Cell(8, 2).Value = totalValue;
                    worksheet.Cell(8, 2).Style.NumberFormat.Format = "#,##0.00";
                    worksheet.Cell(8, 2).Style.Font.Bold = true;
                    worksheet.Cell(8, 2).Style.Font.FontColor = XLColor.FromHtml("#059669");

                    // Table headers
                    int tableRow = 10;
                    var headers = new[] { "ID", "Title", "Author(s)", "Publish Year", "Quantity", "Price", "Total Value" };
                    for (int col = 1; col <= headers.Length; col++)
                    {
                        var cell = worksheet.Cell(tableRow, col);
                        cell.Value = headers[col - 1];
                        cell.Style.Font.Bold = true;
                        cell.Style.Font.FontColor = XLColor.White;
                        cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#1E293B");
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    }

                    // Table data
                    tableRow++;
                    foreach (var book in bookList.OrderBy(b => b.Title))
                    {
                        worksheet.Cell(tableRow, 1).Value = book.BookId;
                        worksheet.Cell(tableRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        worksheet.Cell(tableRow, 2).Value = book.Title;

                        worksheet.Cell(tableRow, 3).Value = book.DisplayAuthorNames ?? "Unknown";

                        worksheet.Cell(tableRow, 4).Value = book.PublishYear;

                        var quantityCell = worksheet.Cell(tableRow, 5);
                        quantityCell.Value = book.Quantity;
                        quantityCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        quantityCell.Style.Font.Bold = true;

                        // Color code quantity
                        if (book.Quantity == 0)
                        {
                            quantityCell.Style.Font.FontColor = XLColor.FromHtml("#94A3B8");
                        }
                        else if (book.Quantity > 0 && book.Quantity <= 5)
                        {
                            quantityCell.Style.Font.FontColor = XLColor.FromHtml("#EF4444");
                        }
                        else
                        {
                            quantityCell.Style.Font.FontColor = XLColor.FromHtml("#059669");
                        }

                        var priceCell = worksheet.Cell(tableRow, 6);
                        priceCell.Value = book.Price;
                        priceCell.Style.NumberFormat.Format = "#,##0.00";
                        priceCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        var totalValueCell = worksheet.Cell(tableRow, 7);
                        totalValueCell.Value = (long)book.Quantity * book.Price;
                        totalValueCell.Style.NumberFormat.Format = "#,##0.00";
                        totalValueCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        totalValueCell.Style.Font.Bold = true;

                        // Alternating row colors
                        if (tableRow % 2 == 0)
                        {
                            for (int col = 1; col <= 7; col++)
                            {
                                worksheet.Cell(tableRow, col).Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FAFC");
                            }
                        }

                        // Add borders
                        for (int col = 1; col <= 7; col++)
                        {
                            worksheet.Cell(tableRow, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        }

                        tableRow++;
                    }

                    // Set column widths
                    worksheet.Column(1).Width = 8;
                    worksheet.Column(2).Width = 30;
                    worksheet.Column(3).Width = 20;
                    worksheet.Column(4).Width = 20;
                    worksheet.Column(5).Width = 12;
                    worksheet.Column(6).Width = 15;
                    worksheet.Column(7).Width = 15;

                    workbook.SaveAs(filePath);
                }

                MessageBox.Show($"Inventory exported successfully to:\n{filePath}", "Export Success", MessageBoxButton.OK, MessageBoxImage.Information);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting inventory: {ex.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private string GetMonthName(int month)
        {
            return month switch
            {
                1 => "January",
                2 => "February",
                3 => "March",
                4 => "April",
                5 => "May",
                6 => "June",
                7 => "July",
                8 => "August",
                9 => "September",
                10 => "October",
                11 => "November",
                12 => "December",
                _ => "Unknown"
            };
        }
    }
}

