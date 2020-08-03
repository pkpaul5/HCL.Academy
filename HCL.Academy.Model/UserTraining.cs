using System.ComponentModel.DataAnnotations;

namespace HCL.Academy.Model
{
    public class UserTraining
    {
        // For AcademyJoinersTraining List
        [Display(Name = "EmployeeName")]
        public string Employee { get; set; }
        public string EmployeeId { get; set; }
        public string EmailId { get; set; }
        public string Role { get; set; }
        public int SkillId { get; set; }// Course of the training like Java, PEGA etc
        public string SkillName { get; set; }
        public string ProjectName { get; set; }
        public int TrainingId { get; set; } // Name of the Training
        public string TrainingName { get; set; }
        public bool IsMandatory { get; set; }
        [Display(Name = "CompletionStatus")]
        public bool IsTrainingCompleted { get; set; }
        public bool IsTrainingActive { get; set; }
        public bool IsIncludeOnBoarding { get; set; }
        public string LastDayCompletion { get; set; }
        public string CompletedDate { get; set; }
        public string StatusColor { get; set; }
        public string ItemStatus { get; set; }
        public int UserId { get; set; }
        public int Id { get; set; }

        public string AdminApprovalStatus { get; set; }

    }
}
