using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Milestone3WebApp.Models
{
    public class SkillProgression
    {
        [Key]
        public int ID { get; set; } // Primary key
        public required string ProficiencyLevel { get; set; } // Required field
        public required int LearnerID { get; set; } // Foreign key to Learner
        public required string SkillName { get; set; } // Foreign key to Skill
        public required DateTime Timestamp { get; set; } // Required field (Timestamp as DateTime)

        // Navigation properties
        public Learner Learner { get; set; }
        public Skill Skill { get; set; } // Navigation property to Skill (composite foreign key)
    }
}