namespace Milestone3WebApp.Models
{
    public class SubmitFeedbackViewModel
    {
        public int ActivityID { get; set; }
        public TimeSpan Timestamp { get; set; }
        public string EmotionalState { get; set; }
    }
}