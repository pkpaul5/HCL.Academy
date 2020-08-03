namespace HCL.Academy.Model
{
    public class TrainingMasterRequest:RequestBase
    {
        public int Id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string skillType { get; set; }
        public string trainingCategory { get; set; }
        public string trainingLink { get; set; }
        public string selectedContent { get; set; }
        public string document { get; set; }
    }
}
