using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
namespace WebsiteDownTracker
{
    public class TelegramService : IMessageService
    {
        private readonly ILogger<TelegramService> _logger;
        private readonly AppSettings _appSettings;
        public TelegramService(ILogger<TelegramService> logger, IOptions<AppSettings> appSettings)
        {
            _logger = logger;
            _appSettings = appSettings.Value;
        }
        public void SendMessage(string message)
        {
            try
            {
                _logger.LogInformation("SendMessage is called with message: {message}", message);
                var botClient = new TelegramBotClient(_appSettings.TelegramBotKey);
                var result = botClient.SendTextMessageAsync(_appSettings.TelegramChatId, message, disableWebPagePreview: true).Result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception on SendMessage");
            }
        }
    }
}
