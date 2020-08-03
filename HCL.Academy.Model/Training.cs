namespace HCL.Academy.Model
{
    public class Training
    {
        public int TrainingId { get; set; }
        public int SkillId { get; set; }
        public int CompetencyId { get; set; }
        public string TrainingName { get; set; }
        public bool IsMandatory { get; set; }
    }
}
