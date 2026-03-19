using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using BookStoreManagement.API.Models.Entities;

namespace BookStoreManagement.API.Models.Entities
{
    [Table("users")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("user_id")]
        public int UserId {get; set;}

        [Required]
        [MaxLength(100)]
        [Column("username")]
        public string Username {get; set; } = string.Empty;

        [Required]
        [Column("password_hash")]
        public string PasswordHash {get; set;} = string.Empty;

        [MaxLength(200)]
        [Column("full_name")]
        public string? FullName {get; set;}

        [Column("role_id")]
        public string RoleId {get; set;} = "staff";

        [Column("created_at")]
        public DateTime CreatedAt {get; set;} = DateTime.UtcNow;

        [Column("reset_token")]
        public string? ResetToken { get; set; }

        [Column("reset_token_expires")]
        public DateTime? ResetTokenExpires { get; set; }

        [Column("email")]
        public string Email { get; set; } = string.Empty;

        public virtual Employee? Employee { get; set; }

    }
}
