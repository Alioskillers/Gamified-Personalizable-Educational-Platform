using Microsoft.AspNetCore.Identity;

namespace Milestone3WebApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Add extra fields for your user model if needed
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public string ProfilePicture { get; set; }
    }
}