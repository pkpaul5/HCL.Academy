using System;

namespace HCL.Academy.Model
{
    public class RoleAssignmentRequest:RequestBase
    {
        public string Email { get; set; }
        public string UserId { get; set; }
        public string RoleId { get; set; }
        public bool Ismandatory { get; set; }
        public DateTime Lastdayofcompletion { get; set; }
    }
}
