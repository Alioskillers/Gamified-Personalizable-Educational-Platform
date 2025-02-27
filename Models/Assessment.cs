using System;

namespace Milestone3WebApp.Models
{
    public class Assessment
    {
        public int ID { get; set; } // Primary key
        public required string AssessmentName { get; set; } // Required field
        public required int ModuleID { get; set; } // Foreign key to Module
        public required int CourseID { get; set; } // Foreign key to Course
        public required string Type { get; set; } // Required field (Type as string)
        public required int TotalMarks { get; set; } // Required field
        public required int PassingMarks { get; set; } // Required field
        public required string Criteria { get; set; } // Required field
        public required decimal Weightage { get; set; } // Required field
        public required string Description { get; set; } // Required field
        public required string Title { get; set; } // Required field

        // Navigation properties
        public Module Module { get; set; } // Navigation property to Module
        public Course Course { get; set; } // Navigation property to Course
    }
}