using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Milestone3WebApp.Models
{
    public class Ranking
    {
        [Key]
        public required int BoardID { get; set; } // Foreign key to Leaderboard
        public required int LearnerID { get; set; } // Foreign key to Learner
        public required int CourseID { get; set; } // Foreign key to Course
        public required int Rank { get; set; } // Rank (Required field)
        public required int TotalPoints { get; set; } // Total points (Required field)

        // Navigation properties
        public Leaderboard Leaderboard { get; set; } // Navigation property to Leaderboard
        public Learner Learner { get; set; } // Navigation property to Learner
        public Course Course { get; set; } // Navigation property to Course
    }
}