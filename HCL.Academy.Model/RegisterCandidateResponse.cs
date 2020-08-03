namespace HCL.Academy.Model
{
    public class RegisterCandidateResponse
    {
        public int statusCode { get; set; }
        public ResponseBody responseBody { get; set; }
    }
    public class ResponseBody
    {
        public string username { get; set; }
        public string message { get; set; }
        
    }
}
