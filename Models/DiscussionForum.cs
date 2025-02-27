using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Milestone3WebApp.Models
{
    public class DiscussionForum
    {
        [Key]
        public int ForumID { get; set; } // Primary key
        public required int ModuleID { get; set; } // Foreign key to Module
        public required int CourseID { get; set; } // Foreign key to Course
        public required string Post { get; set; } // Required field (Post as string)
        public required string Title { get; set; } // Required field (Title as string)
        public required DateTime LastActive { get; set; } // Required field (LastActive as DateTime)
        public required DateTime Timestamp { get; set; } // Required field (Timestamp as DateTime)
        public required string Description { get; set; } // Required field (Description as string)

        // Navigation properties
        public Module Module { get; set; } // Navigation property to Module
        public Course Course { get; set; } // Navigation property to Course
    }
}