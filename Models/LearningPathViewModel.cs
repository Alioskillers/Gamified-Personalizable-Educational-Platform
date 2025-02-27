using System.ComponentModel.DataAnnotations;

namespace Milestone3WebApp.Models
{
    public class LearningPathViewModel
    {
        public int PathID { get; set; } // Optional: Primary key to identify the path

        public int LearnerID { get; set; } // ID of the learner associated with the learning path

        public int ProfileID { get; set; } // ID of the personalization profile associated with the path

        public string CompletionStatus { get; set; } // Status of the path (e.g., "Completed", "In Progress")

        public string CustomContent { get; set; } // Additional content for the learning path

        public string AdaptiveRules { get; set; } // Rules for adapting the learning path
    }
}