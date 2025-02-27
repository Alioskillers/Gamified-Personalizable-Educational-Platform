using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Milestone3WebApp.Models
{
    public class Instructor
    {
        [Key]
        public int InstructorID { get; set; } // Primary key
        public required string Name { get; set; } // Required field
        public required string LatestQualification { get; set; } // Required field
        public required string ExpertiseArea { get; set; } // Required field
        public required string Email { get; set; } // Required field
    }
}