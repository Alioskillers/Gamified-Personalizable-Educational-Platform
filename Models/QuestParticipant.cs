using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Milestone3WebApp.Models
{
    public class QuestParticipant
    {
        [Key]
        public required int QuestID { get; set; } // Foreign key to Quest
        public required int LearnerID { get; set; } // Foreign key to Learner

        // Navigation properties
        public Quest Quest { get; set; } // Navigation property to Quest
        public Learner Learner { get; set; } // Navigation property to Learner
    }
}