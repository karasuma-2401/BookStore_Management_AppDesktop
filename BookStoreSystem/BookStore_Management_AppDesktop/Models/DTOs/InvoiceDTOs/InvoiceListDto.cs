using System;

namespace BookStore_Management_AppDesktop.Models.DTOs.InvoiceDTOs
{
    public class InvoiceListDto
    {
        public int InvoiceId { get; set; }        // Khớp invoice_id
        public DateTime InvoiceDate { get; set; } // Khớp invoice_date
        public decimal Total { get; set; }        // Khớp total (NUMERIC)
        public string? CustomerName { get; set; } // Backend lấy từ bảng Customers
        public int? CustomerId { get; set; }
        public string? StaffName { get; set; }    // Backend lấy từ bảng Users
        public int TotalItems { get; set; }       // Backend đếm từ bảng invoice_details
        public string Status { get; set; } = null!; // Khớp string (đã convert từ int ở DB)
    }
}