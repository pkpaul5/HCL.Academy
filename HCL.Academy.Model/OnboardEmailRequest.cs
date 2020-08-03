namespace HCL.Academy.Model
{
    public class OnboardEmailRequest:RequestBase
    {
        public string Email { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Skill { get; set; }
        public int SkillId { get; set; }
        public string CompetencyLevel { get; set; }
    }
}
