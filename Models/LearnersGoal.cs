using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Milestone3WebApp.Models
{
    public class LearnersGoal
    {
        [Key]
        public required int GoalID { get; set; } // Foreign key to LearningGoal
        public required int LearnerID { get; set; } // Foreign key to Learner

        // Navigation properties
        public LearningGoal LearningGoal { get; set; } // Navigation property to LearningGoal
        public Learner Learner { get; set; } // Navigation property to Learner
    }
}