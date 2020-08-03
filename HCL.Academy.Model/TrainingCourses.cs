using System;
using System.Collections.Generic;

namespace HCL.Academy.Model
{
    public class UserSkillDetail
    {
        public string skillName { get; set; }
        public int id { get; set; }
        public bool isDefault { get; set; }
        public CourseStatus skillStatus { get; set; }
        public DateTime lastDayToComplete { get; set; }
        public List<UserTrainingDetail> listOfTraining { get; set; }
        public TrainingType trainingType { get; set; }
    }  
}