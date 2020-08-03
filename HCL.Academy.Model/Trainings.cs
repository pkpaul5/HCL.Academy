using System;
using System.ComponentModel.DataAnnotations;

namespace HCL.Academy.Model
{
    public class UserTrainingDetail:RequestBase
    {
        public int UserId { get; set; }
        public string EmailAddress { get; set; }
        public string UserName { get; set; }

        public string AdminApprovalStatus { get; set; }
        public int Id { get; set; }
        public string SkillName { get; set; }
        public int SkillId { get; set; }
        public string TrainingName { get; set; }
        public int TrainingId { get; set; }
        public string ModuleDesc { get; set; }
        public string DocumentUrl { get; set; }
        public bool Mandatory { get; set; }
        //added for Link purpose
        public bool IsLink { get; set; }
        public string LinkUrl { get; set; }
        //NoOfattempts & lastDaytoComplete & PassedStatus?
        public bool IsWikiLink { get; set; }
        public int NoOfAttempts { get; set; }
        public DateTime LastDayToComplete { get; set; }
        public bool IsTrainingCompleted { get; set; }
        public TraningStatus status { get; set; }
        public Colors bgColor { get; set; }
        public string CompletionDate { get; set; }
        public TrainingType TrainingType { get; set; }

        public string TrainingFlag { get; set; }
        public bool IsAcademyAdmin { get; set; }
        public bool IsProjectAdmin { get; set; }
        public string Progress { get; set; }
        public int ProjectId { get; set; }

        public string AdminName { get; set; }

    }

    public enum TraningStatus
    {
        [Display(Name = "On Going")]
        OnGoing,

        [Display(Name = "Completed")]
        Completed,

        [Display(Name = "Over Due")]
        OverDue,

        [Display(Name = "Failed")]
        Failed
    }

    public enum TrainingType
    {
        SkillTraining,
        RoleTraining
    }

}