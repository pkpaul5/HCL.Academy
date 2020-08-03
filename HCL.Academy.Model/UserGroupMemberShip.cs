using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCL.Academy.Model
{
    public class UserGroupMemberShip
    {
        public string GroupName { get; set; }
        public int GroupId { get; set; }
        public int GroupPermission { get; set; }
    }
}
