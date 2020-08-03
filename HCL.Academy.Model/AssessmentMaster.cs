using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HCL.Academy.Model
{
    public class AssessmentMaster
    {
        public int AssessmentId { get; set; }
        [Required(ErrorMessage = "AssessmentName is Required")]
        public string AssessmentName { get; set; }
        public string Description { get; set; }
        public List<Training> Trainings { get; set; }
        public string SelectedTraining { get; set; }
        public int? SelTrainingId { get; set; }
        public bool IsMandatory { get; set; }
        public string AssessmentLink { get; set; }
        [Required(ErrorMessage = "AssessmentTimeInMins is Required")]
        public int AssessmentTimeInMins { get; set; }
        public List<Skill> Skills { get; set; }
        public string SelectedSkill { get; set; }
        public int SelSkillId { get; set; }
        public List<Competence> Competencies { get; set; }
        public string SelectedCompetency { get; set; }
        public int SelCompetencyId { get; set; }
        [Required(ErrorMessage = "PassingMarks is Required")]
        public int PassingMarks { get; set; }
        [Required(ErrorMessage = "Points is Required")]
        public int Points { get; set; }
    }
}
