using System.Collections.Generic;

namespace HCL.Academy.Model
{
    public  class AssignTrainingRequest:RequestBase
    {
        public List<UserTraining> UserTraining { get; set; }
        public int UserId { get; set; }
    }
}
