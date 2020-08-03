namespace HCL.Academy.Model
{
    public class RoleTrainingRequest :RequestBase
    {
        public int ItemId { get; set; }
        public int TrainingId { get; set; }
        public int RoleId { get; set; }
        public bool IsMandatory { get; set; }
    }
}
