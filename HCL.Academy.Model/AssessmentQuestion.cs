using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace HCL.Academy.Model
{
    public class AssessmentQuestion
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "SelectedAssessmentId is Required")]
        public int SelectedAssessmentId { get; set; }
        [Required(ErrorMessage = "Question is Required")]
        [DataType(DataType.MultilineText)]
        public string Question { get; set; }
        [Required(ErrorMessage = "Marks is Required")]
        public int Marks { get; set; }
        [DisplayName("Assessment")]
        public string SelectedAssessment { get; set; }
        [Required(ErrorMessage = "Option1 is Required")]
        public string Option1 { get; set; }
        [Required(ErrorMessage = "Option2 is Required")]
        public string Option2 { get; set; }
        [Required(ErrorMessage = "Option3 is Required")]
        public string Option3 { get; set; }
        [Required(ErrorMessage = "Option4 is Required")]
        public string Option4 { get; set; }
        public string Option5 { get; set; }
        [Required(ErrorMessage = "CorrectOption is Required")]
        public string CorrectOption { get; set; }
        [Required(ErrorMessage = "CorrectOptionSequence is Required")]
        public int CorrectOptionSequence { get; set; }
        public List<AssessmentMaster> Assessments { get; set; }              
    }
}
