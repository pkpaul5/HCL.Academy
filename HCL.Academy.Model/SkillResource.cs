using System.ComponentModel.DataAnnotations;

namespace HCL.Academy.Model
{
    public class SkillResource
    {
        public string skill { get; set; }
        public int skillId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        [Display(Name = "Novice Count")]
        public int beginnerCount { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        [Display(Name = "Advanced Beginner Count")]
        public int advancedBeginnerCount { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        [Display(Name = "Competent Count")]
        public int competentCount { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        [Display(Name = "Proficient Count")]
        public int proficientCount { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        [Display(Name = "Expert Count")]
        public int expertCount { get; set; }
    }
}