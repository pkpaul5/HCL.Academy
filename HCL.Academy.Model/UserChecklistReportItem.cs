using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCL.Academy.Model
{
    public class UserChecklistReportItem
    {
        public string CheckListItem { get; set; }
        public string CheckListStatus { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string EmployeeID { get; set; }
        public string CompletionDate { get; set; }
        public string OnboardingDate { get; set; }
    }
}
