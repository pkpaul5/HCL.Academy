namespace HCL.Academy.Model
{
    public  class DeleteSkillTrainingRequest:RequestBase
    {
        public int id { get; set; }
        public string skill { get; set; }
        public string competency { get; set; }

    }
}
