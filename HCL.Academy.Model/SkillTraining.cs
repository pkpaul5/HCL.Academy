using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HCL.Academy.Model
{
    public class SkillTraining
    {
        public int id { get; set; }
        [Required(ErrorMessage = "Name is Required")]
        public string trainingName { get; set; }
        [Required(ErrorMessage = "Training is Required")]
        public string trainingId { get; set; }
        public List<Skill> skills { get; set; }
        public List<Competence> competences { get; set; }
        public List<SkillCompetencyLevelTraining> trainings { get; set; }
        [Required(ErrorMessage = "Skill is Required")]
        public string selectedSkill { get; set; }
        [Required(ErrorMessage = "Competence is Required")]
        public string selectedCompetence { get; set; }
        [Required(ErrorMessage = "Training is Required")]
        public string selectedTraining { get; set; }
        public string competency { get; set; }
        [Required(ErrorMessage = "Competence is Required")]
        public int competencyLevelId { get; set; }
        public string skill { get; set; }
        [Required(ErrorMessage = "Skill is Required")]
        public int skillId { get; set; }
        public string assessment { get; set; }
        public List<Assessment> assessments { get; set; }
        public List<GEO> GEOs { get; set; }
        [Required(ErrorMessage = "GEO is Required")]
        public string selectedGEO { get; set; }
        public int points { get; set; }
        public string GEO { get; set; }
        [Required(ErrorMessage = "GEO is Required")]
        public int GEOId { get; set; }
        public bool isMandatory { get; set; }
        public bool isAssessmentRequired { get; set; }
        public int? assessmentId { get; set; }
        public string description { get; set; }
        public string trainingLink { get; set; }
        public string trainingDocument { get; set; }
        public string trainingCategory { get; set; }
        public string SkillType { get; set; }

    }
}