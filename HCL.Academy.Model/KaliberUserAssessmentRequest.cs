using System.Collections.Generic;

namespace HCL.Academy.Model
{
    public class KaliberUserAssessmentRequest
    {
        public string emailID { get; set; }
        public string accountCode { get; set; }
        public string skillLevel { get; set; }
        public string technology { get; set; }

        public string assessmentName { get; set; }
        public string ragStatus { get; set; }
        public string mcqScore { get; set; }
        public string mcqResult { get; set; }
        public string codeReviewExamScore { get; set; }
        public string buildStatus { get; set; }
        public string totalTests { get; set; }
        public string testStatus { get; set; }
        public string testCaseSuccessPercentage { get; set; }
        public string testCoveragePercentage { get; set; }
    }

    public class KaliberUserAssessmentCollectionRequest
    {
        public int statusCode { get; set; }
        public List<KaliberUserAssessmentRequest> kaliberuserassessmentrequest { get; set; }
    }
}
