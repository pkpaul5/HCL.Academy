using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCL.Academy.Model
{
    public class KalibreActivationResponse
    {
        public int statusCode { get; set; }
        public KalibreActivationResponseBody responseBody { get; set; }
    }
}
