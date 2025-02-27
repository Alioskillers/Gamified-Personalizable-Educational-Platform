namespace Milestone3WebApp.Models{
    public class CreateAssessmentViewModel
{
    public string AssessmentName { get; set; }
    public int ModuleID { get; set; }
    public int CourseID { get; set; }
    public string Type { get; set; }
    public int TotalMarks { get; set; }
    public int PassingMarks { get; set; }
    public string Criteria { get; set; }
    public decimal Weightage { get; set; }
    public string Description { get; set; }
    public string Title { get; set; }
}
}