using System.Collections.Generic;

namespace HCL.Academy.Model
{
   
    public class CheckListItem
    {
        public int id { get; set; }
        public string name { get; set; }
        
        public string internalName { get; set; }
        
        public string desc { get; set; }
        
        public string choice { get; set; }
        public string geoName { get; set; }
        public int geoId { get; set; }

        public string roleName { get; set; }
        public int roleId { get; set; }
    }
}