using System.ComponentModel.DataAnnotations;

namespace Milestone3WebApp.Models
{
    public class InstructorEditViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string LatestQualification { get; set; }
        public string ExpertiseArea { get; set; }
        [EmailAddress]
        public string Email { get; set; }
    }
}