namespace Milestone3WebApp.Models{
    public class LearningActivityViewModel
{
    public int ActivityID { get; set; }
    public int CourseID { get; set; }
    public int ModuleID { get; set; }
    public string ActivityType { get; set; }
    public string InstructionDetails { get; set; }
    public int MaxPoints { get; set; }
}
}