
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HCL.Academy.Model
{
    public class Project
    {
        public int id { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9'' ']+$", ErrorMessage = "Project name cannot contain special characters")]
        [Required(ErrorMessage = "Project Name is Required")]
        public string projectName { get; set; }
        public int parentProjectId { get; set; }
        public int projectLevel { get; set; }

        public List<Skills> lstSkills { get; set; }
        public List<ProjectAdmin> projectAdmins { get; set; }
        public List<Project> childProjects { get; set; }
        
    }
    

}