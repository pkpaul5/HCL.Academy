using System.Collections.Generic;

namespace HCL.Academy.Model
{
    public class OnBoardingViewModel
    {
        public List<OnBoarding> topRowList { get; set; }
        public List<OnBoarding> bottomRowList { get; set; }

        public List<bgColor> bgColorList { get; set; }
        public bool sendEmail { get; set; }
    }
}