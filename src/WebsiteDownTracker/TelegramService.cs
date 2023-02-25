using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
namespace WebsiteDownTracker
{
    public class TelegramService : ITelegramService
    {
        private readonly ILogger<TelegramService> _logger;
        private readonly IConfiguration _configuration;
        public string botKey { get; set; }
        public int chatId { get; set; }
        public TelegramService(ILogger<TelegramService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            botKey = _configuration.GetValue<string>("TelegramBotKey");
            chatId = _configuration.GetValue<int>("TelegramChatId");
        }
        public void SendMessage(string message)
        {
            try
            {
                _logger.LogInformation("SendMessage is called with message: {message} and chatId: {chatId}", message, chatId);
                var botClient = new TelegramBotClient(this.botKey);
                var result = botClient.SendTextMessageAsync(this.chatId, message, disableWebPagePreview: true).Result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception on SendMessage");
            }
        }
    }
}
