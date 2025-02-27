using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Milestone3WebApp.Models
{
    public class LearningActivity
    {
        [Key]
        public int ActivityID { get; set; } // Primary key

        // Required fields
        public required string ActivityType { get; set; }
        public required string InstructionDetails { get; set; }
        public required int MaxPoints { get; set; }

        // Foreign keys
        public required int ModuleID { get; set; }
        public required int CourseID { get; set; }

        // Navigation properties
        public Module Module { get; set; }
        public Course Course { get; set; }
    }
}