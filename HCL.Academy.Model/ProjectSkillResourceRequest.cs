namespace HCL.Academy.Model
{
    public class ProjectSkillResourceRequest : RequestBase
    {        
        public int projectId { get; set; }
        public int skillId { get; set; }
        public int competencyLevelId { get; set; }
        public int expectedResourceCount { get; set; }
        public int availableResourceCount { get; set; }
    }
}
