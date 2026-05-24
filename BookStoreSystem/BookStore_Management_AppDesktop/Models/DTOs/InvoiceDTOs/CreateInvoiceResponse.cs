using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore_Management_AppDesktop.Models.DTOs.InvoiceDTOs
{
    public class CreateInvoiceResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public int? InvoiceId { get; set; }
    }
}
