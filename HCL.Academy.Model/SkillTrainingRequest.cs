using System.Collections.Generic;

namespace HCL.Academy.Model
{
    public  class SkillTrainingRequest:RequestBase
    {
        public int id { get; set; }
        
        public string trainingName { get; set; }
        public List<Skill> skills { get; set; }
        public List<Competence> competences { get; set; }
        public List<SkillCompetencyLevelTraining> trainings { get; set; }
        
        public string selectedSkill { get; set; }
        
        public string selectedCompetence { get; set; }
        
        public string selectedTraining { get; set; }
        public string competency { get; set; }
        public string skill { get; set; }
        public string assessment { get; set; }
        public List<Assessment> assessments { get; set; }
        public List<GEO> GEOs { get; set; }
        
        public string selectedGEO { get; set; }
        public int points { get; set; }
        public string GEO { get; set; }
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
