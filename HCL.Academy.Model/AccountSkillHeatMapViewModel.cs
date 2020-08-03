using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HCL.Academy.Model
{
    public class AccountSkillHeatMapViewModel
    {
        public int competencyLevel { get; set; }
        public int projectId { get; set; }
        public List<SkillCompetencyResource> resources;
        public IEnumerable<SelectListItem> competencies { get; set; }
    }
}
