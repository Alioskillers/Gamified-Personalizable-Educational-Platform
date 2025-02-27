using System;

namespace Milestone3WebApp.Models
{
    public class LearnerProfilePicture
    {
        public int ProfilePictureID { get; set; } // Primary Key
        public int LearnerID { get; set; }        // Foreign Key referencing the learner table
        public string FilePath { get; set; }      // Path to the uploaded file
        public DateTime UploadedAt { get; set; }  // Timestamp for the upload
    }
}