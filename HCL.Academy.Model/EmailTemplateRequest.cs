namespace HCL.Academy.Model
{
    public class EmailTemplateRequest:RequestBase
    {
        public int id { get; set; }
        public string title { get; set; }
        public string emailSubject { get; set; }
        public string emailBody { get; set; }
    }
}
