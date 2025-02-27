using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Milestone3WebApp.Models
{
    public class LearnerBadge
    {
        [Key]
        public required int LearnerID { get; set; } // Foreign key to Learner
        public required int BadgeID { get; set; } // Foreign key to Badge

        // Navigation properties
        public Learner Learner { get; set; } // Navigation property to Learner
        public Badge Badge { get; set; } // Navigation property to Badge
    }
}