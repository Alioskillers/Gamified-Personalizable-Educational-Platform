using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Milestone3WebApp.Models
{
    public class LearnerDiscussion
    {
        [Key]
        public required int ForumID { get; set; } // Foreign key to DiscussionForum
        public required int LearnerID { get; set; } // Foreign key to Learner
        public required string Post { get; set; } // Required field (Post content)
        public required DateTime Time { get; set; } // Required field (Time of post)

        // Navigation properties
        public DiscussionForum DiscussionForum { get; set; } // Navigation property to DiscussionForum
        public Learner Learner { get; set; } // Navigation property to Learner
    }
}