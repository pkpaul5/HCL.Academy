namespace HCL.Academy.Model
{
    public class AssessmentQuestionRequest: RequestBase
    {
        public int ID { get; set; }
        public int SelectedAssessmentId { get; set; }
        public string SelectedAssessment { get; set; }
        public string Question { get; set; }
        public string CorrectOption { get; set; }
        public int CorrectOptionSequence { get; set; }
        public int Marks { get; set; }
        public string Option1 { get; set; }
        public string Option2 { get; set; }
        public string Option3 { get; set; }
        public string Option4 { get; set; }
        public string Option5 { get; set; }
    }
}
