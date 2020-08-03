using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCL.Academy.Model
{
    public class RoleSkill
    {
        public int RoleId { get; set; }
        public int SkillId { get; set; }
        public int CompetencyLevelID { get; set; }
        public string Role { get; set; }
        public string Skill { get; set; }
        public string CompetencyLevel { get; set; }
        public List<Competence> ValidCompetencies { get; set; }
    }
}
