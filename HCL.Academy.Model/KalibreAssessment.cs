using System.Collections.Generic;

namespace HCL.Academy.Model
{
    public class KalibreAssmntAssgmnt
    {
        public string emailID { get; set; }
        public string accountCode { get; set; }
        public List<KalibreAssignment> assessments { get; set; }
    }
}
