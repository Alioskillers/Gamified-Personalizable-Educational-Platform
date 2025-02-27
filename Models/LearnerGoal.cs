using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Milestone3WebApp.Models
{
    public class LearnerGoal
    {
        [Key]
        public required int LearnerID { get; set; } // Foreign key to Learner
        public required int GoalID { get; set; } // Foreign key to LearningGoal

        // Navigation properties
        public Learner Learner { get; set; } // Navigation property to Learner
        public LearningGoal LearningGoal { get; set; } // Navigation property to LearningGoal
    }
}