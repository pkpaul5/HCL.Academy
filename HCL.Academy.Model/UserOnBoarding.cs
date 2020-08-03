using System.Collections.Generic;

namespace HCL.Academy.Model
{
    public class UserOnBoarding
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<GEO> GEOs { get; set; }
        public List<Role> Roles { get; set; }
        public List<Skill> Skills { get; set; }
        public List<Competence> Competence { get; set; }
        public List<Assessment> Assessments { get; set; }
        public List<Training> Trainings { get; set; }
        public List<Status> Status { get; set; }
        public List<BGVStatus> BGVStatus { get; set; }
        public List<ProfileSharing> ProfileSharingStatus { get; set; }
        public string CurrentSkill { get; set; }
        public string CurrentCompetance { get; set; }
        public string CurrentStatus { get; set; }
        public string CurrentProfileSharing { get; set; }
        public string CurrentBGVStatus { get; set; }
        public string CurrentGEO { get; set; }
        public string CurrentRole { get; set; }
        //public bool IsNLWorkPermit { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }

        public List<UserTraining> UserTrainings { get; set; }
        public List<UserAssessment> UserAssessments { get; set; }

        public List<UserSkill> UserSkills { get; set; }
        public bool IsPresentInOnBoard { get; set; }
        public List<Project> Projects { get; set; }
        public int EmployeeId { get; set; }
        public bool Active { get; set; }
    }
}