using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCL.Academy.Model
{
    public class EventRequest:RequestBase
    {
        public AcademyEvent eventinfo { get; set; }
    }
}
