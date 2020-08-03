namespace HCL.Academy.Model
{
    public class UserSkill
    {
        public int Id { get; set; }
        public string Employee { get; set; }
        public string Skill { get; set; }
        public string Competence { get; set; }
        public string SkillwiseCompetencies { get; set; }
        public string SkillwiseCompetencyIds { get; set; }
        public string ProfessionalSkills { get; set; }
        public string SoftSkills { get; set; }
        public int SkillId { get; set; }
        public string LastDayCompletion { get; set; }
        public int CompetenceId { get; set; }
        public int UserId { get; set; }
        public bool IsMandatory { get; set; }
        public bool CompetencyChanged { get; set; }
        public int RoleId { get; set; }
    }
}
