using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Milestone3WebApp.Models
{
    public class LearningGoal
    {
        [Key]
        public int ID { get; set; } // Primary key

        // Required fields
        public required string Status { get; set; }
        public required DateTime Deadline { get; set; }
        public required string Description { get; set; }
    }
}