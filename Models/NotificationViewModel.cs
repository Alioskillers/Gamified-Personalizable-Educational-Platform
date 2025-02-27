namespace Milestone3WebApp.Models{
public class NotificationViewModel
{
    public int ID { get; set; }
    public string Message { get; set; }
    public DateTime Timestamp { get; set; }
    public string UrgencyLevel { get; set; }
    public string LearnerName { get; set; }
}
}