using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCL.Academy.Model
{
    public class TrainingCompletionRequest:RequestBase
    {
        public List<string> trainingDetails { get; set; }
        public string AdminApprovalStatus { get; set; }
    }
}
