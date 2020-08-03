using System.ComponentModel.DataAnnotations;

namespace HCL.Academy.Model
{
    public class RSSFeedMaster
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Title is required")]
        public string Title
        {
            get;

            set;

        }

        [Required(ErrorMessage = "Title Node is required.")]
        public string TitleNode
        {
            get;

            set;

        }

        [Required(ErrorMessage = "RSS Feed Url is required")]
        public string RSSFeedUrl
        {
            get;

            set;

        }

        public string DescriptionNode
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
        public string hrfTitleNodePath { get; set; }
    }
}
