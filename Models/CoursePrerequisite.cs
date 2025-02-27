using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Milestone3WebApp.Models
{
    public class CoursePrerequisite
    {
        [Key]
        public required int CourseID { get; set; } // Foreign key to Course
        public required int PrerequisiteCourseID { get; set; } // Foreign key to Prerequisite Course

        // Navigation properties
        public Course Course { get; set; } // Navigation property to Course
        public Course PrerequisiteCourse { get; set; } // Navigation property to Prerequisite Course
    }
}