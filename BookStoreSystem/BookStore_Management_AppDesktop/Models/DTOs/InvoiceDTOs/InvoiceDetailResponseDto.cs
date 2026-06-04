using System.Collections.Generic;

namespace BookStore_Management_AppDesktop.Models.DTOs.InvoiceDTOs
{
    public class InvoiceDetailResponseDto : InvoiceListDto
    {
        public string? VoucherCode { get; set; } // Khớp string (VoucherCode thay vì ID)
        public List<InvoiceItemDto> Items { get; set; } = new();
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
    }
}