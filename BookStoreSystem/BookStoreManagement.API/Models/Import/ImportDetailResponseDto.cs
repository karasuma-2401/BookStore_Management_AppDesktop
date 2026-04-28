public class ImportDetailResponseDto
{
    public int BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;

    public int Quantity { get; set; }
    public decimal ImportPrice { get; set; }
}