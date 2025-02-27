using System;
using System.Collections.Generic;

namespace Milestone3WebApp.Models
{
    public class Course
    {
        public int CourseID { get; set; } // Primary key

        // Required fields
        public required string Title { get; set; }
        public required string LearningObjective { get; set; }
        public required int CreditPoints { get; set; }
        public required int DifficultyLevel { get; set; }
        public string? PreRequisites { get; set; } // Nullable field
        public required string Description { get; set; }

        // Navigation property
        public ICollection<Module> Modules { get; set; } = new List<Module>();
    }
}