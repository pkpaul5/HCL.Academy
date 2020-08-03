using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
namespace HCL.Academy.Model
{
    public class AcademyEvent
    {
        
        public int id { get; set; }
        [Required(ErrorMessage = "Title is required")]
        [DisplayName("Title")]
        public string title { get; set; }
        [Required(ErrorMessage = "Start Date is required")]
        [DataType(DataType.DateTime)]       
        [DisplayName("StartDate")]
        public DateTime eventDate { get; set; }
        [Required(ErrorMessage = "End Date is required")]
        [DataType(DataType.DateTime)]        
        [DisplayName("EndDate")]
        public DateTime endDate { get; set; }
        [Required(ErrorMessage = "Location is required")]
        [DisplayName("Location")]
        public string location { get; set; }
        [Required(ErrorMessage = "Description is required")]
        [DisplayName("Description")]
        public string description { get; set; }
    }
}