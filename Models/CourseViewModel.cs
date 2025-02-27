namespace Milestone3WebApp.Models
{
    public class CourseViewModel
    {
        public int CourseID { get; set; } // Maps to c.courseID
        public string CourseTitle { get; set; } // Maps to c.title
        public string CourseDescription { get; set; } // Maps to c.description
        public DateTime EnrollmentDate { get; set; } // Maps to ce.enrollment_date
        public string EnrollmentStatus { get; set; } // Maps to ce.status
    }
}