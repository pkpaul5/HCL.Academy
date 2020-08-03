namespace HCL.Academy.Model
{
    public class ProjectSkillResource
    {
        public string projectName { get; set; }
        public int projectId { get; set; }
        public string skill { get; set; }
        public int skillId { get; set; }
        public string competencyLevel { get; set; }
        public int competencyLevelId { get; set; }
        public int expectedResourceCount { get; set; }
        public int availableResourceCount { get; set; }
    }
}
