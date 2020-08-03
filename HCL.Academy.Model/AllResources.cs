using System.ComponentModel.DataAnnotations;

namespace HCL.Academy.Model
{
    public class AllResources
    {
        public string skill { get; set; }
        public int skillId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        [Display(Name = "Novice Count")]
        public int expectedBeginnerCount { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        [Display(Name = "Novice Count")]
        public int availableBeginnerCount { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        [Display(Name = "Advanced Beginner Count")]
        public int expectedAdvancedBeginnerCount { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        [Display(Name = "Advanced Beginner Count")]
        public int availableAdvancedBeginnerCount { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        [Display(Name = "Competent Count")]
        public int expectedCompetentCount { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        [Display(Name = "Competent Count")]
        public int availableCompetentCount { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        [Display(Name = "Proficient Count")]
        public int expectedProficientCount { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        [Display(Name = "Proficient Count")]
        public int availableProficientCount { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        [Display(Name = "Expert Count")]
        public int expectedExpertCount { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        [Display(Name = "Expert Count")]
        public int availableExpertCount { get; set; }
    }
}