using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Milestone3WebApp.Models
{
    public class EmotionalFeedbackReview
    {
        [Key]
        public required int FeedbackID { get; set; } // Foreign key to EmotionalFeedback
        public required int InstructorID { get; set; } // Foreign key to Instructor

        // Required field
        public required string Feedback { get; set; }

        // Navigation properties
        public EmotionalFeedback EmotionalFeedback { get; set; }
        public Instructor Instructor { get; set; }
    }
}