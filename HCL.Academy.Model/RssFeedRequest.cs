namespace HCL.Academy.Model
{
    public class RssFeedRequest:RequestBase
    {
        public int ID { get; set; }
        
        public string Title
        {
            get;

            set;

        }

        
        public string TitleNode
        {
            get;

            set;

        }
      
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
