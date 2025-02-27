using System;
using System.ComponentModel.DataAnnotations;

namespace Milestone3WebApp.Models
{
    public class LearningGoalViewModel
    {
        [Key]
        public int ID { get; set; }

        public string Status { get; set; }

        public DateTime Deadline { get; set; }

        public string Description { get; set; }
    }
}