using System;
using System.ComponentModel.DataAnnotations;

namespace Milestone3WebApp.Models
{
    public class NotifyUpcomingGoalsViewModel
    {
        [Required(ErrorMessage = "Deadline is required.")]
        public DateTime Deadline { get; set; }

        [Required(ErrorMessage = "Learner ID is required.")]
        public int LearnerID { get; set; }

        [Required(ErrorMessage = "Message is required.")]
        public string Message { get; set; }

        [Required(ErrorMessage = "Urgency Level is required.")]
        public string UrgencyLevel { get; set; }
    }
}