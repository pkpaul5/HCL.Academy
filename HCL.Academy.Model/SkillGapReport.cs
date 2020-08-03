using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCL.Academy.Model
{
    public class SkillGapReport
    {
        public int RoleId { get; set; }
        public string EmployeeName { get; set; }
       public int EmployeeID { get; set; }
       public string EmailID { get; set; }
        public string Skill { get; set; }
        public string ExpectedCompetencyLevel { get; set; }
        public string ActualCompetencyLevel { get; set; }
        public List<Role> Roles { get; set; }
    }
}
