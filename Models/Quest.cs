using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Milestone3WebApp.Models
{
    public class Quest
    {
        [Key]
        public int QuestID { get; set; } // Primary key
        public required string DifficultyLevel { get; set; } // Required field
        public required string Criteria { get; set; } // Required field
        public required string Description { get; set; } // Required field
        public required string Title { get; set; } // Required field
        public required DateTime Deadline { get; set; } // Required field
    }
}