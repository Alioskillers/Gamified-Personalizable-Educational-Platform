using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Milestone3WebApp.Models
{
    public class Teaches
    {
        [Key]
        public required int InstructorID { get; set; } // Foreign key to Instructor
        public required int CourseID { get; set; } // Foreign key to Course

        // Navigation properties
        public Instructor Instructor { get; set; } // Navigation property to Instructor
        public Course Course { get; set; } // Navigation property to Course
    }
}