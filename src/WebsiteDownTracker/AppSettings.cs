namespace WebsiteDownTracker
{
    public class AppSettings
    {
        public string Websites { get; set; }
        public int Delay { get; set; }
        public int Timeout { get; set; }
        public string TelegramBotKey { get; set; }
        public long TelegramChatId { get; set; }
        public int DownCountLimit { get; set; }
        
    }
}
