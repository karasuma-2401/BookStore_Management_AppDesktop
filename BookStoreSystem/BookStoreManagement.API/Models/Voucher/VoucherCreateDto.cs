namespace BookStoreManagement.API.Models.Voucher
{
    public class VoucherCreateDto
    {
        public string Code { get; set; } = null!;
        public int? DiscountPercent { get; set; }
        public decimal? DiscountAmount { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int? UsageLimit { get; set; }
    }
}
