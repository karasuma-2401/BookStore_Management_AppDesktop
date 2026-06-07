public class ImportDetailResponseDto
{
    public int BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public int PublishYear { get; set; }

    public int Quantity { get; set; }
    public decimal ImportPrice { get; set; }
    /// <summary>
    /// Giá bán của mỗi cuốn sách tại thời điểm nhập (= ImportPrice * priceRate/GIABAN do user cấu hình).
    /// Lưu cố định trong import_details để truy vết lịch sử giá.
    /// </summary>
    public decimal SellingPrice { get; set; }
    public decimal TotalPrice => Quantity * ImportPrice;
}
