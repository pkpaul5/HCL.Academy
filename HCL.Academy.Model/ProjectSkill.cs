using System.Collections.Generic;

namespace HCL.Academy.Model
{
    public class ProjectDetails
    {
        public int projectId { get; set; }
        public string projectName { get; set; }

        public List<ProjectSkill> projectSkill { get; set; }
    }
    public class ProjectSkill
    {
        public string project { get; set; }
        public string skill { get; set; }
        public int projectId { get; set; }
        public int skillId { get; set; }
        public int itemId { get; set; }
    }
}