namespace Milestone3WebApp.Models{
public class AdminNotificationViewModel
{
    public int NotificationID { get; set; }
    public DateTime Timestamp { get; set; }
    public string Message { get; set; }
    public string UrgencyLevel { get; set; }
    public int AdminID { get; set; } // Include AdminID
}
}