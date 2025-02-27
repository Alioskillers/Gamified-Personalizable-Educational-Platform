using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Milestone3WebApp.Models
{
    public class CourseEnrollment
    {
        [Key]
        public int EnrollmentID { get; set; } // Primary key

        // Foreign Keys
        public required int CourseID { get; set; }
        public required int LearnerID { get; set; }

        // Required fields
        public required DateTime CompletionDate { get; set; }
        public required DateTime EnrollmentDate { get; set; }
        public required string Status { get; set; }

        // Navigation properties
        public Course Course { get; set; }
        public Learner Learner { get; set; }
    }
}