using System;
using System.ComponentModel.DataAnnotations;

namespace HCL.Academy.Model
{
    public class News
    {
        public int ID { get; set; }
        [Display(Name = "Image URL")]
        public string imageURL
        {
            get;

            set;

        }
        [Display(Name = "Header")]
        public string header
        {
            get;

            set;

        }
        [Display(Name = "Body")]
        public string body
        {
            get;

            set;

        }
        [Display(Name = "Trimmed Body")]
        public string trimmedBody
        {
            get;

            set;

        }        

    }
}