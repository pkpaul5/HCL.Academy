using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace HCL.Academy.Model
{
    public class EmailTemplate
    {
        public int id { get; set; }
        [Required(ErrorMessage ="Title is required.")]
        public string title { get; set; }
        [Required(ErrorMessage ="Subject is required.")]
        public string emailSubject { get; set; }
        [Required(ErrorMessage ="Body is required.")]
        [AllowHtml]
        public string emailBody { get; set; }
    }
}
