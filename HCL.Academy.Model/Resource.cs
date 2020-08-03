using System.Collections.Generic;

namespace HCL.Academy.Model
{
    public class Resource
    {
        public string projectName { get; set; }
        public int projectId { get; set; }
        
        public List<AllResources> allResources { get; set; }
    }
}