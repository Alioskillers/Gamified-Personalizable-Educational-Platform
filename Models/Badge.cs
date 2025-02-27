namespace Milestone3WebApp.Models
{
    public class Badge
    {
        public int BadgeID { get; set; } // Primary key

        // Required fields
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Criteria { get; set; }
        public required int Points { get; set; }
    }
}