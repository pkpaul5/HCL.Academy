namespace HCL.Academy.Model
{
    public class UserTrainingsRequest:RequestBase
    {
        public int SkillId { get; set; }
        public int CompetenceId { get; set; }

        public int ProjectId { get; set; }

    }
}
