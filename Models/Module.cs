using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Milestone3WebApp.Models
{
    public class Module
    {
        [Key]
        public int ModuleID { get; set; } // Primary key

        // Required fields
        public required string Title { get; set; }
        public required int Difficulty { get; set; }
        public required string ContentURL { get; set; }
        
        // Foreign Key
        public required int CourseID { get; set; }
        public Course Course { get; set; } // Navigation property

        // Navigation property
        public ICollection<LearningActivity> LearningActivities { get; set; } = new List<LearningActivity>();
    }
}