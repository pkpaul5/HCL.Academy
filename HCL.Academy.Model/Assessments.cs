using System.Collections.Generic;

namespace HCL.Academy.Model
{
    public class Assessments
    {
        public int assessmentId { get; set; }
        public string assessmentName { get; set; }  // Form AcademyModules
        public List<QuestionDetail> questionDetails { get; set; } //From AcademyAssessments
        public string passingMarks { get; set; }  // From AcademyModules
        public string assessmentStatus { get; set; }  // From
        public string securedMarks { get; set; }
        public int totalMarks { get; set; }
        public bool maxAttemptsExceeded { get; set; }
        public bool assessmentCompletionStatus { get; set; }
        public int passingPercentage { get; set; }
        public List<AcademyJoinersCompletion> assessmentDetails { get; set; }

    }
    public class AssesmentResult
    {
        public int securedMarks { get; set; }
        public int assessmentId { get; set; }
        public int totalMarks { get; set; }
        public int passingPercentage { get; set; }
    }

    
}