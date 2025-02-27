namespace Milestone3WebApp.Models
{
    public class AchievementViewModel
    {
        public int LearnerID { get; set; }
        public int BadgeID { get; set; }
        public string Description { get; set; }
        public DateTime DateEarned { get; set; }
        public string Type { get; set; }
    }
}