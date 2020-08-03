namespace HCL.Academy.Model
{
    public class RoleTraining
    {
        public int TrainingId { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string TrainingName { get; set; }
        public bool IsMandatory { get; set; }
        public string URL { get; set; }
        public int Points { get; set; }
        public string Description { get; set; }
        public int RoleTrainingId { get; set; }

    }
}
