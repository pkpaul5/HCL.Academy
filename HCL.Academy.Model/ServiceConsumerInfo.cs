using System.Collections.Generic;
using System.Net;

namespace HCL.Academy.Model
{
    public class ServiceConsumerInfo
    {
        public int id { get; set; }
        public string emailId { get; set; }
        public string name { get; set; }
        public ICredentials spCredential { get; set; }
        public int spUserId { get; set; }
        public List<string> Groups { get; set; }
        public int GroupPermission { get; set; }
    }
}
