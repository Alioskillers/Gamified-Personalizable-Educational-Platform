namespace Milestone3WebApp.Models
{
    public class TrendsFilterModel
    {
        public int CourseID { get; set; }
        public int ModuleID { get; set; }
        public DateTime TimePeriod { get; set; } = DateTime.Now.AddDays(-7); // Default: last 7 days
        public bool IsInstructorView { get; set; } // If true, use InstructorEmotionalTrendAnalysis proc
    }
}