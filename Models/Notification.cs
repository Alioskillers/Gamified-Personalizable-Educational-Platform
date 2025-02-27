using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Milestone3WebApp.Models
{
    public class Notification
    {
        [Key]
        public int ID { get; set; } // Primary key
        public required DateTime Timestamp { get; set; } // Required field (Timestamp as DateTime)
        public required string UrgencyLevel { get; set; } // Required field (UrgencyLevel as string)
        public required string Message { get; set; } // Required field (Message as string)
    }
}