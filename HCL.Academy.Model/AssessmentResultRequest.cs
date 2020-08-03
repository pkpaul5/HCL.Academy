using System.Collections.Generic;

namespace HCL.Academy.Model
{
    public class AssessmentResultRequest:RequestBase
    {
        public AssesmentResult Result { get; set; }
        public List<QuestionDetail> QuestionDetails { get; set; }
    }
}
