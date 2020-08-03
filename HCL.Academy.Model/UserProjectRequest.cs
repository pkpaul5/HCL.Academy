namespace HCL.Academy.Model
{
    public class UserProjectRequest : RequestBase
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public int SkillId { get; set; }
        public int UserId { get; set; }

    }
}
