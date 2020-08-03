using System.ComponentModel.DataAnnotations;

namespace HCL.Academy.Model
{

    public class RSSFeed
    {
        public int ID { get; set; }
        [Required(ErrorMessage ="Ttitle is required")]
        public string Title
        {
            get;

            set;

        }

        [Required(ErrorMessage ="Title Node is required.")]
        public string TitleNode
        {
            get;

            set;

        }

        [Required(ErrorMessage ="Link node is required")]
        public string LinkNode
        {
            get;

            set;

        }

        public string DescriptionNode
        {
            get;

            set;

        }

        public string TrimedDescription
        {
            get;

            set;

        }

        public string PubDateNode
        {
            get;

            set;

        }
        
        public string itemNodePath { get; set; }
        public int rssFeedOrder { get; set; }
    }
}