using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCL.Academy.Model
{
    public class NewsRequest:RequestBase
    {
        public int ID { get; set; }
        public String imageURL
        {
            get;

            set;

        }

        public string header
        {
            get;

            set;

        }

        public string body
        {
            get;

            set;

        }

        public string trimmedBody
        {
            get;

            set;

        }
    }
}
