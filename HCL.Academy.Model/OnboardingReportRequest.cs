namespace HCL.Academy.Model
{
    public class OnboardingReportRequest:RequestBase
    {
        public string Status { get; set; }
        public bool IsExcelDownload { get; set; }
        public int RoleId { get; set; }
        public int GEOId { get; set; }
        public int ProjectId { get; set; }
        public string option { get; set; }
        public string search { get; set; }
    }
}
