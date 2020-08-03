namespace HCL.Academy.Model
{
    public class Assessment
    {
        public int AssessmentId { get; set; }
        public string AssessmentName { get; set; }
        public string Description { get; set; }
        public int TrainingId { get; set; }
        public bool IsMandatory { get; set; }
        public string AssessmentLink { get; set; }

        public int AssessmentTimeInMins { get; set; }
        public int SkillId { get; set; }
        public int CompetencyId { get; set; }
    }
}
