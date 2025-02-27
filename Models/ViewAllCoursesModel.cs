namespace Milestone3WebApp.Models
{
    public class ViewAllCoursesModel
    {
        public int CourseID { get; set; }
        public string Title { get; set; }
        public string LearningObjective { get; set; }
        public int CreditPoints { get; set; }
        public int DifficultyLevel { get; set; }
        public string Prerequisites { get; set; }
        public string Description { get; set; }
    }
}