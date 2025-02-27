using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Milestone3WebApp.Models
{
    public class SurveyQuestion
    {
        [Key]
        public required int SurveyID { get; set; } // Foreign key to Survey
        public required string Question { get; set; } // Required field (Question as string)

        // Navigation property
        public Survey Survey { get; set; } // Navigation property to Survey
    }
}