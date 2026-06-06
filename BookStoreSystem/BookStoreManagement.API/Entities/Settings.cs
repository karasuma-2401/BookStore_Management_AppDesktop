using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BookStoreManagement.API.Models.Entities
{
    [Table("settings")]
    public class Settings
    {
        [Key]
        [Column("setting_name")]
        [MaxLength(100)]
        public string SettingName { get; set; } = null!;

        [Required]
        [Column("value")]
        [MaxLength(100)]
        public string Value { get; set; } = null!;
    }
}