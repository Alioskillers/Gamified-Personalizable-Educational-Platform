using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Milestone3WebApp.Models
{
    [Table("Users")] // Maps this class to the Users table in the database
    public class User
    {
        [Key] // Marks this as the primary key
        [Column("username")] // Maps this property to the username column
        public int Username { get; set; }

        [Required] // Specifies that this field is required
        [Column("password")] // Maps this property to the password column
        [MaxLength(255)] // Restricts the maximum length for this field
        public string Password { get; set; }

        [Required] // Specifies that this field is required
        [Column("user_type")] // Maps this property to the user_type column
        [MaxLength(50)] // Restricts the maximum length for this field
        public string UserType { get; set; }

        public virtual ProfilePicture ProfilePicture { get; set; }
    }
}