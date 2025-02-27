using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Milestone3WebApp.Models
{
    public class TargetTrait
    {
        [Key]
        public required int ModuleID { get; set; } // Foreign key to Module
        public required int CourseID { get; set; } // Foreign key to Course

        // Required field
        public required string Trait { get; set; }

        // Navigation properties
        public Module Module { get; set; }
        public Course Course { get; set; }
    }
}