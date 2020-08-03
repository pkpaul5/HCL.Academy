using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HCL.Academy.Model
{
    public class ExternalUser
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "EmployeeID is required")]
        public int EmployeeId { get; set; }
        public string UserId { get; set; }
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Email address is not valid")]
        [Required(ErrorMessage = "Email address is required")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        public string Group { get; set; }
        [Display(Name = "Group")]
        [Required(ErrorMessage = "Group is required")]
        public int GroupId { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string EncryptedPassword { get; set; }
        [Compare("Password")]
        [Display(Name = "Confirm Password")]        
        public string ConfimredPassword { get; set; }
        [Display(Name = "Password")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "Password must be 8 characters")]
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        [Display(Name = "Organization")]
        [Required(ErrorMessage = "Organization is required")]
        public int OrganizationID { get; set; }
        public string Organization { get; set; }
        [Display(Name = "Skill")]
        [Required(ErrorMessage ="Skill is required")]
        public int SkillId { get; set; }
        [Display(Name = "Competency")]
        [Required(ErrorMessage = "Competency is required")]
        public int CompetencyLevelId { get; set; }
        public string GEO { get; set; }
        [Display(Name = "GEO")]
        [Required(ErrorMessage = "GEO is required")]
        public int GEOId { get; set; }
        public string Role { get; set; }
        [Display(Name = "Role")]
        [Required(ErrorMessage = "Role is required")]
        public int RoleId { get; set; }
        public bool FirstPasswordChanged { get; set; }
    }
}
