namespace Milestone3WebApp.Models{
public class ProfilePicture
{
    public int Id { get; set; }
    public string Username { get; set; } // Foreign key
    public string FilePath { get; set; }
    public DateTime UploadedAt { get; set; }

    public virtual User User { get; set; } // Navigation property
}
}