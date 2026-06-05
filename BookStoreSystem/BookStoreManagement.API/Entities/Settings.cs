using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BookStoreManagement.API.Models.Entities
{
    public class Settings
    {
        [Key]
        [MaxLength(100)]
        public string SettingName { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Value { get; set; } = null!;
    }
}