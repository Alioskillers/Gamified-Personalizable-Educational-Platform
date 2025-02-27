using System;

namespace Milestone3WebApp.Models
{
    public class AnalyticsViewModel
    {
        public int LearnerID { get; set; } // ID of the learner
        public int CourseID { get; set; } // ID of the course
        public int ModuleID { get; set; } // ID of the module
        public int AverageScore { get; set; } // Average score of the learner in the module/course
    }
}