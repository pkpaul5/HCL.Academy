using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCL.Academy.Model
{
    public class ProjectAdmin
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public int EmployeeId { get; set; }
        public int ProjectId { get; set; }
    }
}
