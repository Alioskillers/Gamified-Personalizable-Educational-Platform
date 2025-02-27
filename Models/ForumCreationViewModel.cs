namespace Milestone3WebApp.Models{
    public class ForumCreationViewModel{
public int ForumID { get; set; } // Primary key
        public int ModuleID { get; set; } // Foreign key to modules
        public int CourseID { get; set; } // Foreign key to modules
        public string? Post { get; set; } // Content of the forum post
        public string Title { get; set; } // Title of the forum
        public DateTime LastActive { get; set; } // Last active timestamp
        public DateTime Timestamp { get; set; } // Creation timestamp
        public string? Description { get; set; }
    }
}