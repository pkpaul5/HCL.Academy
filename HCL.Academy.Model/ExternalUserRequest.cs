using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCL.Academy.Model
{
    public class ExternalUserRequest:RequestBase
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }      
        public string UserName { get; set; }        
        public string Name { get; set; }        
        public string EncryptedPassword { get; set; }        
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public int OrganizationId { get; set; }        
        public int GroupId { get; set; }
        public int RoleId { get; set; }
        public int SkillId { get; set; }
        public int CompetencyLevelId { get; set; }
        public int GEOId { get; set; }

    }
}
