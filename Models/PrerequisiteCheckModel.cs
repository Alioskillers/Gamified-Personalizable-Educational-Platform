namespace Milestone3WebApp.Models
{
    public class PrerequisiteCheckModel
    {
        public int LearnerID { get; set; } // ID of the learner checking prerequisites
        public int CourseID { get; set; } // ID of the course to check prerequisites for
    }
}