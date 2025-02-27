using System;

namespace Milestone3WebApp.Models
{
    public class ContentLibrary
    {
        public int ID { get; set; } // Primary key
        public required int ModuleID { get; set; } // Foreign key to Module
        public required int CourseID { get; set; } // Foreign key to Course
        public required string Title { get; set; } // Required field
        public required string Description { get; set; } // Required field
        public required string Metadata { get; set; } // Required field
        public required string Type { get; set; } // Required field
        public required string ContentUrl { get; set; } // Required field

        // Navigation properties
        public Module Module { get; set; } // Navigation property to Module
        public Course Course { get; set; } // Navigation property to Course
    }
}