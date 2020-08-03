using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCL.Academy.Model
{
    public class ProjectAdminInfo
    {
        public bool IsFirstLevelAdmin { get; set; }
        public bool IsSecondLevelAdmin { get; set; }
        public List<int> SecondLevelProjects { get; set; }

        public bool IsThirdLevelAdmin { get; set; }
        public List<ProjectInfo> ThirdLevelProjects { get; set; }
    }
}
