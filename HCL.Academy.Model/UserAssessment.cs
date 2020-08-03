namespace HCL.Academy.Model
{
    public class UserAssessment
    {
        public int TrainingAssessmentId { get; set; } // Assessment Id
        public string TrainingAssessment { get; set; } // Assessment Name
        public int SkillId { get; set; } // Course Id like Java etc
        public string SkillName { get; set; }
        public int TrainingId { get; set; } // Trainging Id
        public string TrainingName { get; set; } // Training Name
        public bool IsMandatory { get; set; }
        public bool IsAssessmentComplete { get; set; }
        public bool IsAssessmentActive { get; set; }
        public bool IsIncludeOnBoarding { get; set; }
        public string LastDayCompletion { get; set; }
        public string CompletedDate { get; set; }

        public string StatusColor { get; set; }
        public string ItemStatus { get; set; }

        public string Employee { get; set; }

        public decimal MarksInPercentage { get; set; }
        public int UserId { get; set; }
        public int Id { get; set; }
    }
}
