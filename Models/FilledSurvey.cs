using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Milestone3WebApp.Models
{
    public class FilledSurvey
    {
        [Key]
        public required int SurveyID { get; set; } // Foreign key to SurveyQuestion
        public required string Question { get; set; } // Foreign key to SurveyQuestion
        public required int LearnerID { get; set; } // Foreign key to Learner
        public required string Answer { get; set; } // Required field (Answer as string)

        // Navigation properties
        public SurveyQuestion SurveyQuestion { get; set; } // Navigation property to SurveyQuestion
        public Learner Learner { get; set; } // Navigation property to Learner
    }
}