using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Milestone3WebApp.Models
{
    public class LearningPath
    {
        [Key]
        public int PathID { get; set; } // Primary key
        public required int LearnerID { get; set; } // Foreign key to Learner
        public required int ProfileID { get; set; } // Foreign key to PersonalizationProfile
        public required string CompletionStatus { get; set; } // Required field
        public required string CustomContent { get; set; } // Required field
        public required string AdaptiveRules { get; set; } // Required field

        // Navigation property to PersonalizationProfile
        public PersonalizationProfile PersonalizationProfile { get; set; }
    }
}