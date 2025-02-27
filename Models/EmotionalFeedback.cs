using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Milestone3WebApp.Models
{
    public class EmotionalFeedback
    {
        [Key]
        public int FeedbackID { get; set; } // Primary key

        // Foreign Keys
        public required int ActivityID { get; set; } // Foreign key to LearningActivity
        public required int LearnerID { get; set; } // Foreign key to Learner

        // Required fields
        public required DateTime Timestamp { get; set; }
        public required string EmotionalState { get; set; }

        // Navigation properties
        public LearningActivity LearningActivity { get; set; }
        public Learner Learner { get; set; }
    }
}