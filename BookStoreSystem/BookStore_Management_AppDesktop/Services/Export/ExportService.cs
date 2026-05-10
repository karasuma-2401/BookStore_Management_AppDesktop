using BookStore_Management_AppDesktop.Models.DTOs.ReportDTOs;
using ClosedXML.Excel;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

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
