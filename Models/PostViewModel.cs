using System;

namespace Milestone3WebApp.Models
{
    public class PostViewModel
    {
        public int ModuleID { get; set; }
        public int CourseID { get; set; }
        public string Post { get; set; }
        public string Title { get; set; }
        public DateTime? LastActive { get; set; } = DateTime.UtcNow; // Set last_active to current time initially
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;   // Set timestamp to current time
        public string Description { get; set; }
    }
}