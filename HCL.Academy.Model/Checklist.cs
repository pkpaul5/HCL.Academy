using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace HCL.Academy.Model
{
    public class Checklist
    {
        public int id { get; set; }
        [Required(ErrorMessage = "Name is Required")]
        public string name { get; set; }
        [Required(ErrorMessage = "Internal Name is Required")]
        public string internalName { get; set; }
        [Required(ErrorMessage = "Description is Required")]
        public string desc { get; set; }
        [Required(ErrorMessage = "Choice is Required")]
        public bool choice { get; set; }
        public List<GEO> GEOs { get; set; }
        [Required(ErrorMessage = "Please select GEO")]
        public string selectedGEO { get; set; }
        public List<Role> roles { get; set; }
        [Required(ErrorMessage = "Please select Role")]
        public string selectedRole { get; set; }


    }
}