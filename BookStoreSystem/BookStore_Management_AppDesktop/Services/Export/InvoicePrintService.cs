using BookStore_Management_AppDesktop.Models.DTOs.InvoiceDTOs;
using BookStore_Management_AppDesktop.Services.API;
using ClosedXML.Excel;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using MessageBox = BookStore_Management_AppDesktop.Views.Windows.CustomMessageBox;

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
                    // 1. Generate path and filename safely
                    var fileName = $"Invoice_{invoice.InvoiceId}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                    var downloadsPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                    downloadsPath = Path.Combine(downloadsPath, "Downloads");

                    if (!Directory.Exists(downloadsPath))
                    {
                        Directory.CreateDirectory(downloadsPath);
                    }

                    var filePath = Path.Combine(downloadsPath, fileName);

                    // 2. Create Excel Workbook using ClosedXML
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Receipt");

                        // FIX LINE 42: Show grid lines correctly
                        worksheet.ShowGridLines = true;

                        // Set explicit column widths
                        worksheet.Column(1).Width = 8;   // No.
                        worksheet.Column(2).Width = 35;  // Item Title
                        worksheet.Column(3).Width = 10;  // Qty
                        worksheet.Column(4).Width = 18;  // Unit Price
                        worksheet.Column(5).Width = 20;  // Total

                        // 3. Header Store Information
                        var storeCell = worksheet.Cell("A1");
                        storeCell.Value = "BOOKSTORE MANAGEMENT";
                        storeCell.Style.Font.Bold = true;
                        storeCell.Style.Font.FontSize = 16;
                        worksheet.Range("A1:E1").Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        worksheet.Cell("A2").Value = "UIT, Linh Trung, Thu Duc, HCMC";
                        worksheet.Range("A2:E2").Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        worksheet.Range("A2:E2").Style.Font.FontSize = 10;

                        worksheet.Cell("A3").Value = "Tel: 0123.456.789";
                        worksheet.Range("A3:E3").Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        worksheet.Range("A3:E3").Style.Font.FontSize = 10;

                        // Receipt Main Title
                        var titleCell = worksheet.Cell("A5");
                        titleCell.Value = "RECEIPT";
                        titleCell.Style.Font.Bold = true;
                        titleCell.Style.Font.FontSize = 14;
                        worksheet.Range("A5:E5").Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        // 4. Invoice Metadata Section
                        worksheet.Cell("A7").Value = "Invoice ID:";
                        worksheet.Cell("B7").Value = $"#{invoice.InvoiceId}";
                        worksheet.Cell("B7").Style.Font.Bold = true;

                        worksheet.Cell("A8").Value = "Created Date:";
                        worksheet.Cell("B8").Value = invoice.InvoiceDate.ToString("MM/dd/yyyy HH:mm");

                        worksheet.Cell("A9").Value = "Cashier:";
                        // FIX LINE 82: Changed from EmployeeName to StaffName
                        worksheet.Cell("B9").Value = invoice.StaffName ?? "N/A";

                        worksheet.Cell("A10").Value = "Customer:";
                        worksheet.Cell("B10").Value = invoice.CustomerName ?? "Retail Customer";

                        worksheet.Cell("A11").Value = "Voucher Applied:";
                        worksheet.Cell("B11").Value = !string.IsNullOrEmpty(invoice.VoucherCode) ? invoice.VoucherCode : "None";

                        worksheet.Range("A7:A11").Style.Font.Italic = true;

                        // 5. Table Headers Definition
                        int currentRow = 13;
                        worksheet.Cell(currentRow, 1).Value = "No.";
                        worksheet.Cell(currentRow, 2).Value = "Item Title / Description";
                        worksheet.Cell(currentRow, 3).Value = "Qty";
                        worksheet.Cell(currentRow, 4).Value = "Unit Price";
                        worksheet.Cell(currentRow, 5).Value = "Total";

                        var headerRange = worksheet.Range(currentRow, 1, currentRow, 5);
                        headerRange.Style.Font.Bold = true;
                        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                        headerRange.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        headerRange.Style.Border.BottomBorder = XLBorderStyleValues.Medium;
                        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        currentRow++;

                        // 6. Populate Purchase Items
                        int itemIndex = 1;
                        foreach (var item in invoice.Items)
                        {
                            worksheet.Cell(currentRow, 1).Value = itemIndex;
                            worksheet.Cell(currentRow, 2).Value = item.BookTitle;
                            worksheet.Cell(currentRow, 3).Value = item.Quantity;
                            worksheet.Cell(currentRow, 4).Value = item.SalePrice;
                            worksheet.Cell(currentRow, 5).Value = item.SubTotal;

                            // Alignments
                            worksheet.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            worksheet.Cell(currentRow, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            worksheet.Cell(currentRow, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                            worksheet.Cell(currentRow, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            // Currency formats
                            worksheet.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0\" đ\"";
                            worksheet.Cell(currentRow, 5).Style.NumberFormat.Format = "#,##0\" đ\"";

                            // Underline data row border
                            var itemRange = worksheet.Range(currentRow, 1, currentRow, 5);
                            itemRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            itemRange.Style.Border.BottomBorderColor = XLColor.LightGray;

                            currentRow++;
                            itemIndex++;
                        }

                        // 7. Summary Blocks
                        currentRow++; // Blank line spacer

                        // Total Amount
                        worksheet.Cell(currentRow, 4).Value = "Total Amount:";
                        // FIX LINE 142: Changed from TotalAmount to Total
                        worksheet.Cell(currentRow, 5).Value = invoice.Total;
                        worksheet.Cell(currentRow, 5).Style.NumberFormat.Format = "#,##0\" đ\"";
                        worksheet.Cell(currentRow, 5).Style.Font.Bold = true;
                        currentRow++;

                        // Voucher / Discount
                        worksheet.Cell(currentRow, 4).Value = "Voucher Code:";
                        // FIX LINE 149: Handled Voucher display instead of missing DiscountAmount
                        worksheet.Cell(currentRow, 5).Value = !string.IsNullOrEmpty(invoice.VoucherCode) ? invoice.VoucherCode : "None";
                        worksheet.Cell(currentRow, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        currentRow++;

                        // Final Due
                        currentRow++;
                        worksheet.Cell(currentRow, 4).Value = "FINAL DUE:";
                        worksheet.Cell(currentRow, 4).Style.Font.Bold = true;
                        // FIX LINE 157: Corrected property to FontSize
                        worksheet.Cell(currentRow, 4).Style.Font.FontSize = 12;

                        // FIX LINE 159: Changed from FinalAmount to Total
                        worksheet.Cell(currentRow, 5).Value = invoice.Total;
                        worksheet.Cell(currentRow, 5).Style.Font.Bold = true;
                        // FIX LINE 161: Corrected property to FontSize
                        worksheet.Cell(currentRow, 5).Style.Font.FontSize = 13;
                        worksheet.Cell(currentRow, 5).Style.NumberFormat.Format = "#,##0\" đ\"";
                        worksheet.Cell(currentRow, 5).Style.Border.BottomBorder = XLBorderStyleValues.Double;

                        // 8. Footer Note
                        currentRow += 3;
                        worksheet.Cell(currentRow, 1).Value = "Thank you for shopping with us!";
                        worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                        worksheet.Cell(currentRow, 1).Style.Font.Italic = true;
                        worksheet.Range(currentRow, 1, currentRow, 5).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        workbook.SaveAs(filePath);
                    }

                    MessageBox.Show($"Invoice exported successfully to:\n{filePath}", "Export Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error exporting invoice: {ex.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            });
        }

        public async Task<bool> ExportPaymentHistoryToExcelAsync(InvoiceDetailResponseDto invoice, System.Collections.Generic.List<PaymentResponseDto> payments)
        {
            if (invoice == null || payments == null)
            {
                MessageBox.Show("No data available to export!", "Export Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return await Task.Run(() =>
            {
                try
                {
                    var fileName = $"PaymentHistory_Invoice_{invoice.InvoiceId}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                    var downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

                    if (!Directory.Exists(downloadsPath))
                    {
                        Directory.CreateDirectory(downloadsPath);
                    }

                    var filePath = Path.Combine(downloadsPath, fileName);

                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Payment History");
                        worksheet.ShowGridLines = true;

                        worksheet.Column(1).Width = 15;  // Payment ID
                        worksheet.Column(2).Width = 25;  // Date
                        worksheet.Column(3).Width = 25;  // Staff
                        worksheet.Column(4).Width = 20;  // Amount

                        var titleCell = worksheet.Cell("A1");
                        titleCell.Value = "PAYMENT HISTORY - INVOICE #" + invoice.InvoiceId;
                        titleCell.Style.Font.Bold = true;
                        titleCell.Style.Font.FontSize = 16;
                        worksheet.Range("A1:D1").Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        worksheet.Cell("A3").Value = "Customer:";
                        worksheet.Cell("B3").Value = invoice.CustomerName ?? "Guest";
                        worksheet.Cell("A4").Value = "Invoice Total:";
                        worksheet.Cell("B4").Value = invoice.Total;
                        worksheet.Cell("B4").Style.NumberFormat.Format = "#,##0\" đ\"";

                        worksheet.Cell("A5").Value = "Total Paid:";
                        worksheet.Cell("B5").Value = invoice.PaidAmount;
                        worksheet.Cell("B5").Style.NumberFormat.Format = "#,##0\" đ\"";
                        worksheet.Cell("B5").Style.Font.Bold = true;

                        worksheet.Cell("A6").Value = "Remaining Due:";
                        worksheet.Cell("B6").Value = invoice.RemainingAmount;
                        worksheet.Cell("B6").Style.NumberFormat.Format = "#,##0\" đ\"";
                        worksheet.Cell("B6").Style.Font.Bold = true;

                        worksheet.Range("A3:A6").Style.Font.Italic = true;

                        int currentRow = 8;
                        worksheet.Cell(currentRow, 1).Value = "Payment ID";
                        worksheet.Cell(currentRow, 2).Value = "Payment Date";
                        worksheet.Cell(currentRow, 3).Value = "Cashier (Staff)";
                        worksheet.Cell(currentRow, 4).Value = "Paid Amount";

                        var headerRange = worksheet.Range(currentRow, 1, currentRow, 4);
                        headerRange.Style.Font.Bold = true;
                        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                        headerRange.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        headerRange.Style.Border.BottomBorder = XLBorderStyleValues.Medium;
                        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        currentRow++;

                        foreach (var payment in payments)
                        {
                            worksheet.Cell(currentRow, 1).Value = payment.PaymentId;
                            worksheet.Cell(currentRow, 2).Value = payment.PaymentDate.ToString("MM/dd/yyyy HH:mm");
                            worksheet.Cell(currentRow, 3).Value = payment.StaffName;
                            worksheet.Cell(currentRow, 4).Value = payment.Amount;

                            worksheet.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            worksheet.Cell(currentRow, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            worksheet.Cell(currentRow, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                            worksheet.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0\" đ\"";

                            var rowRange = worksheet.Range(currentRow, 1, currentRow, 4);
                            rowRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            rowRange.Style.Border.BottomBorderColor = XLColor.LightGray;

                            currentRow++;
                        }

                        workbook.SaveAs(filePath);
                    }

                    MessageBox.Show($"Payment history exported successfully to:\n{filePath}", "Export Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error exporting payment history: {ex.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            });
        }
    }
}