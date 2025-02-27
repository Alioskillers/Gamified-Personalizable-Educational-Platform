namespace Milestone3WebApp.Models
{
    public class EmotionalFeedbackViewModel
    {
        public int FeedbackID { get; set; }
        public int ActivityID { get; set; }
        public int LearnerID { get; set; }
        public DateTime Timestamp { get; set; }
        public string EmotionalState { get; set; }
    }
}