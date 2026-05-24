using BookStore_Management_AppDesktop.Models.DTOs.InvoiceDTOs;
using ClosedXML.Excel;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace BookStore_Management_AppDesktop.Services.Export
{
    public class InvoiceExportService : IInvoiceExportService
    {
        public async Task<bool> ExportInvoiceToExcelAsync(InvoiceDetailResponseDto invoice)
        {
            if (invoice == null)
            {
                MessageBox.Show("No invoice data available to export!", "Export Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return await Task.Run(() =>
            {
                try
                {
                    var fileName = $"Invoice_{invoice.InvoiceId}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                    var downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

                    if (!Directory.Exists(downloadsPath)) Directory.CreateDirectory(downloadsPath);
                    var filePath = Path.Combine(downloadsPath, fileName);

                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Receipt");
                        worksheet.ShowGridLines = true;

                        // Cột
                        worksheet.Column(1).Width = 8; worksheet.Column(2).Width = 35;
                        worksheet.Column(3).Width = 10; worksheet.Column(4).Width = 18;
                        worksheet.Column(5).Width = 20;

                        // Header
                        worksheet.Cell("A1").Value = "BOOKSTORE MANAGEMENT";
                        worksheet.Range("A1:E1").Merge().Style.Font.SetBold().Font.SetFontSize(16).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        worksheet.Cell("A2").Value = "UIT, Linh Trung, Thu Duc, HCMC";
                        worksheet.Range("A2:E2").Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        worksheet.Cell("A5").Value = "RECEIPT";
                        worksheet.Range("A5:E5").Merge().Style.Font.SetBold().Font.SetFontSize(14).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                        // Metadata
                        worksheet.Cell("A7").Value = "Invoice ID:"; worksheet.Cell("B7").Value = $"#{invoice.InvoiceId}";
                        worksheet.Cell("A8").Value = "Created Date:"; worksheet.Cell("B8").Value = invoice.InvoiceDate.ToString("MM/dd/yyyy HH:mm");
                        worksheet.Cell("A9").Value = "Cashier:"; worksheet.Cell("B9").Value = invoice.StaffName ?? "N/A";
                        worksheet.Cell("A10").Value = "Customer:"; worksheet.Cell("B10").Value = invoice.CustomerName ?? "Retail Customer";

                        // Items
                        int currentRow = 13;
                        worksheet.Cell(currentRow, 1).Value = "No."; worksheet.Cell(currentRow, 2).Value = "Item Title";
                        worksheet.Cell(currentRow, 3).Value = "Qty"; worksheet.Cell(currentRow, 4).Value = "Unit Price";
                        worksheet.Cell(currentRow, 5).Value = "Total";
                        worksheet.Range(currentRow, 1, currentRow, 5).Style.Font.SetBold().Fill.SetBackgroundColor(XLColor.LightGray);

                        currentRow++;
                        foreach (var item in invoice.Items)
                        {
                            worksheet.Cell(currentRow, 1).Value = currentRow - 13;
                            worksheet.Cell(currentRow, 2).Value = item.BookTitle;
                            worksheet.Cell(currentRow, 3).Value = item.Quantity;
                            worksheet.Cell(currentRow, 4).Value = item.SalePrice;
                            worksheet.Cell(currentRow, 5).Value = item.SubTotal;
                            currentRow++;
                        }

                        // Summary
                        currentRow++;
                        worksheet.Cell(currentRow, 4).Value = "Total Amount:"; worksheet.Cell(currentRow, 5).Value = invoice.Total;

                        workbook.SaveAs(filePath);
                    }
                    MessageBox.Show("Export success!", "Success");
                    return true;
                }
                catch (Exception ex)
                {
                    File.AppendAllText("export_errors.log", $"{DateTime.Now}: {ex.Message}{Environment.NewLine}");
                    MessageBox.Show($"Error: {ex.Message}");
                    return false;
                }
            });
        }
    }
}