using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Milestone3WebApp.Models
{
    public class PersonalizationProfile
    {
        [Key]
        public required int LearnerID { get; set; } // Foreign key to Learner
        public required int ProfileID { get; set; } // Primary key
        public required string PreferredContentType { get; set; }
        public required string EmotionalState { get; set; }
        public required string PersonalityType { get; set; }

        // Navigation property
        public required Learner Learner { get; set; }
    }
}