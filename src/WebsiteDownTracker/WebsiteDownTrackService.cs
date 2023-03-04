using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RestSharp;
namespace WebsiteDownTracker
{
    public class WebsiteDownTrackService : BackgroundService
    {
        private readonly ILogger<WebsiteDownTrackService> _logger;
        private readonly IConfiguration _configuration;
        private readonly ITelegramService _telegramService;
        public int timeout { get; set; }
        public int delay { get; set; }
        public WebsiteDownTrackService(ILogger<WebsiteDownTrackService> logger, IConfiguration configuration, ITelegramService telegramService)
        {
            _logger = logger;
            _configuration = configuration;
            _telegramService = telegramService;
            this.timeout = _configuration.GetValue<int>("Timeout");
            this.delay = _configuration.GetValue<int>("Delay");
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("WebsiteDownTrackService running at: {time}", DateTimeOffset.Now);
                try
                {
                    WebsiteDownTrack();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception on WebsiteDownTrackService - ExecuteAsync");
                }
                await Task.Delay(TimeSpan.FromMinutes(this.delay), stoppingToken);
            }
        }
        private void WebsiteDownTrack()
        {
            var websites = _configuration.GetValue<string>("Websites").Split(' ').ToList();
            foreach (var websiteItem in websites)
            {
                try
                {
                    var restClient = new RestClient(websiteItem);
                    restClient.Options.UserAgent = "Google Chrome";
                    var restRequest = new RestRequest(string.Empty, Method.Get);
                    restRequest.Timeout = this.timeout * 60 * 1000;
                    var response = restClient.Execute(restRequest);
                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogInformation("Response: {@response}", response);
                        _telegramService.SendMessage($"{websiteItem} is down on {DateTime.UtcNow.ToString()}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception on WebsiteDownTrack");
                }
            }
        }
    }
}
