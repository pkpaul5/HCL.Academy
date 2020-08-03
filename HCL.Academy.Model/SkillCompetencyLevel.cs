namespace HCL.Academy.Model
{
    public class SkillCompetencyLevel
    {
        public int ItemID { get; set; }
        public int CompetencyID { get; set; }
        public string CompetencyName { get; set; }
        public int SkillID { get; set; }
        public string SkillName { get; set; }
        public string Description { get; set; }
        public int CompetencyLevelOrder { get; set; }
        public string ProfessionalSkills { get; set; }
        public string SoftSkills { get; set; }
        public int TrainingCompletionPoints { get; set; }
        public int AssessmentCompletionPoints { get; set; }
    }
}