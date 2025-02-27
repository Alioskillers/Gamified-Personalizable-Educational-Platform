using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Milestone3WebApp.Models
{
    public class LearningPreference
    {
        [Key]
        public required int LearnerID { get; set; } // Foreign key to Learner

        // Required field
        public required string Preference { get; set; }

        // Navigation property
        public Learner Learner { get; set; }
    }
}