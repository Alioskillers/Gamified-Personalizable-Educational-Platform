using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Milestone3WebApp.Models
{
    public class PathReview
    {
        [Key]
        public required int InstructorID { get; set; } // Foreign key to Instructor
        public required int PathID { get; set; } // Foreign key to LearningPath

        // Required field
        public required string Feedback { get; set; }

        // Navigation properties
        public Instructor Instructor { get; set; }
        public LearningPath LearningPath { get; set; }
    }
}