using System.ComponentModel.DataAnnotations;

namespace HCL.Academy.Model
{
    public class AcademyConfig
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Title is Required")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Value is Required")]
        public string Value { get; set; }
    }
}