using System;

namespace Milestone3WebApp.Models
{
    public class AssessmentViewModel
    {
        public int ID { get; set; } // Assessment ID
        public string AssessmentName { get; set; } // Name of the assessment
        public int CourseID { get; set; } // ID of the course
        public int ModuleID { get; set; } // ID of the module
        public string Type { get; set; } // Type of the assessment (e.g., quiz, assignment)
        public int TotalMarks { get; set; } // Total marks for the assessment
        public int PassingMarks { get; set; } // Passing marks required
        public string Description { get; set; } // Description of the assessment
        public int? Score { get; set; } // Score achieved by the learner (optional)
    }
}