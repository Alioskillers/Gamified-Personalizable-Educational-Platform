using System;

namespace Milestone3WebApp.Models
{
    public class AchievementModel
    {
        public int AchievementID { get; set; } // Unique identifier for the achievement
        public int LearnerID { get; set; } // The ID of the learner earning the achievement
        public int BadgeID { get; set; } // The ID of the associated badge
        public string BadgeTitle { get; set; } // The title of the badge
        public string Description { get; set; } // Description of the achievement
        public DateTime DateEarned { get; set; } // Date the achievement was earned
        public string Type { get; set; } // Type/category of the achievement
    }
}