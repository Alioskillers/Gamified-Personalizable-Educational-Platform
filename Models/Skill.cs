using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Milestone3WebApp.Models
{
    public class Skill
    {
        [Key]
        public required int LearnerID { get; set; } // Foreign key to Learner
        public required string SkillName { get; set; } // Required field

        // Navigation property to Learner
        public Learner Learner { get; set; }
    }
}