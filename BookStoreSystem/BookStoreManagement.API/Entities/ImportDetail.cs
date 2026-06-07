using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStoreManagement.API.Models.Entities
{
    [Table("import_details")]
    public class ImportDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id {get; set;}

        [Column("import_id")]
        public int ImportId {get; set;}

        [Column("book_id")]
        public int BookId {get; set;}

        [Column("quantity")]
        public int Quantity {get; set;}

        [Column("import_price", TypeName = "decimal(12,2)")]
        public decimal ImportPrice {get; set;}

        /// <summary>
        /// Giá bán của mỗi cuốn sách tại thời điểm nhập = ImportPrice * priceRate (GIABAN do user cấu hình).
        /// Lưu riêng ở từng dòng import_details để:
        ///   - Truy vết được giá bán tại thời điểm nhập (lịch sử giá).
        ///   - Tránh việc thay đổi GIABAN sau này làm ảnh hưởng ngược tới các đơn nhập cũ.
        /// </summary>
        [Column("selling_price", TypeName = "decimal(12,2)")]
        public decimal SellingPrice { get; set; }

        [ForeignKey("ImportId")]
        public Import Import {get; set;} = null!;

        [ForeignKey("BookId")]
        public Book Book {get; set;} = null!;
    }
}
