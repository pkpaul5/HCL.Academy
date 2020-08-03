using System.Collections.Generic;


namespace HCL.Academy.Model
{
    public class TrainingStatus
    {
        public List<Competence> Competence { get; set; }        
        public List<Skill> Skills { get; set; }

        public List<Project> Projects { get; set; }
    }
}