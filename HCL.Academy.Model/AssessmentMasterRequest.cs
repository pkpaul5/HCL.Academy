namespace HCL.Academy.Model
{
    public class AssessmentMasterRequest:RequestBase
    {
        public int AssessmentId { get; set; }
        public string AssessmentName { get; set; }
        public string Description { get; set; }
        public int? SelTrainingId { get; set; }
        //public string SelectedTraining { get; set; }
        public bool IsMandatory { get; set; }
        public string AssessmentLink { get; set; }
        public int AssessmentTimeInMins { get; set; }
        //public string SelectedSkill { get; set; }
        public int SelSkillId { get; set; }
        public int PassingMarks { get; set; }
        public int SelCompetencyId { get; set; }
        //public string SelectedCompetency { get; set; }
        public int Points { get; set; }         
    }
}
