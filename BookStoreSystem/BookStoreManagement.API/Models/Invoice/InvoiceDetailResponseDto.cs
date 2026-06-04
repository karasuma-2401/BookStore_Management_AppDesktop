namespace BookStoreManagement.API.Models.Invoice
{
    public class InvoiceDetailResponseDto : InvoiceListDto
    {
        public string? VoucherCode { get; set; }
        public List<InvoiceItemDto> Items { get; set; } = new();
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
    }
}
