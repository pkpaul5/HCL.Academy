using System;

namespace HCL.Academy.Model
{
    public class AcademyJoinersCompletion
    {
        public AcademyJoinersCompletion()
        {
            trainingLookupText = string.Empty;
            trainingAssessmentLookUpText = string.Empty;
            skillLookUpText = string.Empty;

        }
        public int id { get; set; }
        public string title { get; set; }
        public int skillLookUpId { get; set; }
        public string skillLookUpText { get; set; }
        public int trainingLookUpId { get; set; }
        public string trainingLookupText { get; set; }
        public int trainingAssessmentLookUpId { get; set; }
        public string trainingAssessmentLookUpText { get; set; }
        public int trainingAssessmentTimeInMins { get; set; }
        public bool isMandatory { get; set; }
        public bool isTrainingLink { get; set; }
        public string trainingLink { get; set; }
        public DateTime lastDayCompletion { get; set; }
        public DateTime completedDate { get; set; }
        public int marksSecured { get; set; }
        public bool certificateMailSent { get; set; }
        public int attempts { get; set; }
        public int maxAttempts { get; set; }
        public bool assessmentStatus { get; set; }
        public string completionDate { get; set; }
    }
}