using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCL.Academy.Model
{
    public class HomeViewModel
    {
        public List<UserTrainingDetail> skillTrainings { get; set; }
        public List<UserTrainingDetail> roleTrainings { get; set; }
        public List<UserCheckList> checklist { get; set; }
        public List<UserSkill> skills { get; set; }
        public List<AcademyJoinersCompletion> assessments { get; set; }
        public List<AcademyEvent> events { get; set; }
        public List<RSSFeed> rssFeed { get; set; }
        public List<News> news { get; set; }
    }
}
