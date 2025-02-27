using System.ComponentModel.DataAnnotations;

namespace Milestone3WebApp.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public int Username { get; set; }  // Username as an integer ID

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public required string Password { get; set; }
    }
}