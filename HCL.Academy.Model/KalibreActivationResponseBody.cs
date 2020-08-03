using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCL.Academy.Model
{
    public class KalibreActivationResponseBody
    {
        public string emailID { get; set; }
        public string message { get; set; }
        public bool candidateExists { get; set; }
    }
}
