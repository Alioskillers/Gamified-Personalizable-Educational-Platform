using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Milestone3WebApp.Models
{
    public class LearnerAssessment
    {
        [Key]
        public required int LearnerID { get; set; } // Foreign key to Learner
        public required int AssessmentID { get; set; } // Foreign key to Assessment
        public int? Score { get; set; } // Optional field for the score

        // Navigation properties
        public Learner Learner { get; set; } // Navigation property to Learner
        public Assessment Assessment { get; set; } // Navigation property to Assessment
    }
}