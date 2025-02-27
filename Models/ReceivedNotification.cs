using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Milestone3WebApp.Models
{
    public class ReceivedNotification
    {
        [Key]
        public required int NotificationID { get; set; } // Foreign key to Notification
        public required int LearnerID { get; set; } // Foreign key to Learner

        // Navigation properties
        public Notification Notification { get; set; } // Navigation property to Notification
        public Learner Learner { get; set; } // Navigation property to Learner
    }
}