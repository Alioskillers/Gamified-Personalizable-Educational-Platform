using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Milestone3WebApp.Models
{
    public class Collaborative
    {
        [Key]
        public required int QuestID { get; set; } // Primary key

        public required DateTime Deadline { get; set; } // Required field (Deadline)
        public required int MaxNumParticipants { get; set; } // Maximum number of participants

        // Navigation property
        public Quest Quest { get; set; }
    }
}