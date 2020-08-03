using System.ComponentModel.DataAnnotations;

namespace HCL.Academy.Model
{
    public class SkillMaster
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Title is Required")]
        public string Title { get; set; }
        public bool IsDefault { get; set; }
    }
}