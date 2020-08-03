namespace HCL.Academy.Model
{
    public class Competence
    {
        public int CompetenceId { get; set; }
        public string CompetenceName { get; set; }
        public int SkillId { get; set; }
        public string SkillName { get; set; }
        public string Description { get; set; }
        public int TrainingCompletionPoints { get; set; }
        public int AssessmentCompletionPoints { get; set; }
        public int CompetencyLevelOrder { get; set; }
    }
}
