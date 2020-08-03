using System.Collections.Generic;

namespace HCL.Academy.Model
{
    public class LearningChecklist
    {
        public List<UserTrainingDetail> SkillBasedTrainings { get; set; }
        public List<UserTrainingDetail> RoleBasedTrainings { get; set; }

    }
}
