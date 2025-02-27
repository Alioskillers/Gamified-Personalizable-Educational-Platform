using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Milestone3WebApp.Models
{
    public class Learner
    {
        [Key]
        public int LearnerID { get; set; } // Primary key

        // Required fields
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required char Gender { get; set; }
        public required DateTime BirthDate { get; set; }
        public required string Country { get; set; }
        public required string CulturalBackground { get; set; }

        // Navigation properties
        public ICollection<CourseEnrollment> CourseEnrollments { get; set; } = new List<CourseEnrollment>();
        public ICollection<PersonalizationProfile> PersonalizationProfiles { get; set; } = new List<PersonalizationProfile>();
        public ICollection<Achievement> Achievements { get; set; } = new List<Achievement>();
    }
}