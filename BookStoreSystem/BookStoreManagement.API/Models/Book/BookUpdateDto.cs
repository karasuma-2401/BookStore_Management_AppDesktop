namespace BookStoreManagement.API.Models.Book
{
    public class BookUpdateDto
    {
        public string Title { get; set; } = string.Empty;

        public List<int> AuthorIds { get; set; } = new();
        public int PublishYear { get; set; }
        public int Quantity { get; set; }
        // Giá bán sách: Bắt buộc phải có ở DTO để phía server cập nhật được giá bán khi edit.
        // Nếu thiếu field này, mọi cập nhật sách sẽ KHÔNG lưu được giá bán -> hiển thị sai.
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string ImagePath { get; set; } = string.Empty;
        public List<int> CategoryIds { get; set; } = new();
    }
}
