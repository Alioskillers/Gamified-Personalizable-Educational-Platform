namespace Milestone3WebApp.Models
{
    public class DiscussionForumViewModel
    {
        public int ForumID { get; set; }
        public int ModuleID { get; set; }
        public int CourseID { get; set; }
        public string Post { get; set; }
        public string Title { get; set; }
        public DateTime? LastActive { get; set; }
        public DateTime Timestamp { get; set; }
        public string Description { get; set; }
    }
}