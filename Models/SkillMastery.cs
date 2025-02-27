using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Milestone3WebApp.Models
{
    public class SkillMastery
    {
        [Key]
        public required int QuestID { get; set; } // Foreign key to Quest
        public required string Skill { get; set; } // Required field (Skill as string)

        // Navigation property
        public Quest Quest { get; set; } // Navigation property to Quest
    }
}