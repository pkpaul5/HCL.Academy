using System.Collections.Generic;

namespace HCL.Academy.Model
{
    public class ProjectResourcesRequest : RequestBase
    {
        public string projectName { get; set; }
        public int projectId { get; set; }

        public List<SkillResource> skillResources { get; set; }
    }
}
