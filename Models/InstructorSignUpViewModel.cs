using System.ComponentModel.DataAnnotations;

namespace Milestone3WebApp.Models
{
    public class InstructorSignUpViewModel
    {
        [Required]
        [Display(Name = "Name")]
        public required string Name { get; set; }

        [Required]
        [Display(Name = "Latest Qualification")]
        public required string LatestQualification { get; set; }

        [Required]
        [Display(Name = "Expertise Area")]
        public required string ExpertiseArea { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public required string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public required string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
        [Display(Name = "Confirm Password")]
        public required string ConfirmPassword { get; set; }
    }
}