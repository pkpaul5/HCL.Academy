using System.ComponentModel.DataAnnotations;

namespace HCL.Academy.Model
{
    public class UserCompetency
    {
        public string competencyLevel { get; set; }
    }

    public enum CompetencyLevelMaster
    {
        [Display(Name = "Advanced Beginner")]
        Advancedbeginner,

        [Display(Name = "Competent")]
        Competent,

        [Display(Name = "Expert")]
        Expert,

        [Display(Name = "Novice")]
        Novice,

        [Display(Name = "Proficient")]
        Proficient
    }
}