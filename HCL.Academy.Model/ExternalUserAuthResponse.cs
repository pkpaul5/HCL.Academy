using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCL.Academy.Model
{
   public class ExternalUserAuthResponse
    {
        public ExternalUser user { get; set; }
        public bool result { get; set; }
        public string errorMessage { get; set; }
    }
}
