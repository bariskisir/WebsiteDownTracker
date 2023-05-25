namespace WebsiteDownTracker
{
    public class WebsiteDownInfo
    {
        public WebsiteDownInfo(string websiteUrl)
        {
            WebsiteUrl = websiteUrl;
            DownCount = 1;
        }
        public string WebsiteUrl { get; set; }
        public int DownCount { get; set; }
    }
}
