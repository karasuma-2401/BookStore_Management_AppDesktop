namespace BookStoreManagement.API.Models.Invoice
{
    public class InvoiceDetailResponseDto : InvoiceListDto
    {
        public string? VoucherCode { get; set; }
        public List<InvoiceItemDto> Items { get; set; } = new();
    }
}
