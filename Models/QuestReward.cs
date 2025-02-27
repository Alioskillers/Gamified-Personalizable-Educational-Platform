using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Milestone3WebApp.Models
{
    public class QuestReward
    {
        [Key]
        public required int RewardID { get; set; } // Foreign key to Reward
        public required int QuestID { get; set; } // Foreign key to Quest
        public required int LearnerID { get; set; } // Foreign key to Learner
        public required DateTime TimeEarned { get; set; } // Required field (Time the reward was earned)

        // Navigation properties
        public Reward Reward { get; set; } // Navigation property to Reward
        public Quest Quest { get; set; } // Navigation property to Quest
        public Learner Learner { get; set; } // Navigation property to Learner
    }
}