using System.Collections.Generic;

namespace HCL.Academy.Model
{
    public class ChecklistRequest:RequestBase
    {
        public int id { get; set; }
        
        public string name { get; set; }
        
        public string internalName { get; set; }
        
        public string desc { get; set; }
        
        public bool choice { get; set; }
        public List<GEO> GEOs { get; set; }
        
        public string selectedGEO { get; set; }
        public List<Role> roles { get; set; }
        
        public string selectedRole { get; set; }
    }
}
