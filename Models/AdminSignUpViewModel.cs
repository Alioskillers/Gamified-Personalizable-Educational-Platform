using System.ComponentModel.DataAnnotations;

namespace Milestone3WebApp.Models
{
    public class AdminSignUpViewModel
    {
        
        public string Name { get; set; } = string.Empty;

        
        public string Password { get; set; } = string.Empty;
    }
}