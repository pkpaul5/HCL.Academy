namespace HCL.Academy.Model
{
    public class SkillAssignmentRequest : RequestBase
    {
        public int SkillId { get; set; }
        public string LastDayCompletion { get; set; }
        public int CompetenceId { get; set; }
        public bool IsMandatory { get; set; }
        public bool CompetencyChanged { get; set; }
        public int UserId { get; set; }
        public string EmailAddress { get; set; }
        public int Id { get; set; }
        public int RoleId { get; set; }
    }
}
