using System;

namespace Milestone3WebApp.Models
{
    public class Achievement
    {
        public int AchievementID { get; set; } // Primary key

        // Foreign Keys
        public required int LearnerID { get; set; } // Foreign key to Learner
        public required int BadgeID { get; set; } // Foreign key to Badge

        // Required fields
        public required string Description { get; set; }
        public required DateTime DateEarned { get; set; }
        public required string Type { get; set; }

        // Navigation properties
        public Learner Learner { get; set; } // Navigation property to Learner
        public Badge Badge { get; set; } // Navigation property to Badge
    }
}