using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HCL.Academy.Model
{
    public class TrainingMaster
    {
        public int Id { get; set; } 
        [Required(ErrorMessage="Title is Required")]
        public string title { get; set; }        
        public string description { get; set; }
        [Required(ErrorMessage = "Skill Type is Required")]
        public string skillType { get; set; }
        [Required(ErrorMessage = "Category is Required")]
        public string trainingCategory { get; set; }
        
        public string trainingLink { get; set; }
        public List<TrainingContent> contents { get; set; }
        public string selectedContent { get; set; }
        
        public string trainingDocument { get; set; }
    }
}
