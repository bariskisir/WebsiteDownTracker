using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;
namespace WebsiteDownTracker
{
    public class WebsiteDownTrackService : BackgroundService
    {
        private readonly ILogger<WebsiteDownTrackService> _logger;
        private readonly IMessageService _telegramService;
        private readonly AppSettings _appSettings;
        private List<WebsiteDownInfo> _websiteDownInfoList { get; set; }
        public WebsiteDownTrackService(ILogger<WebsiteDownTrackService> logger, IMessageService telegramService, IOptions<AppSettings> appSettings)
        {
            _logger = logger;
            _telegramService = telegramService;
            _appSettings = appSettings.Value;
            _websiteDownInfoList = new List<WebsiteDownInfo>();
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
                await Task.Delay(TimeSpan.FromMinutes(_appSettings.Delay), stoppingToken);
            }
        }
        private void WebsiteDownTrack()
        {
            var websites = _appSettings.Websites.Split(' ').ToList();
            foreach (var websiteItem in websites)
            {
                try
                {
                    var restClient = new RestClient(websiteItem);
                    restClient.Options.UserAgent = "Google Chrome";
                    var restRequest = new RestRequest(string.Empty, Method.Get);
                    restRequest.Timeout = _appSettings.Timeout * 60 * 1000;
                    var response = restClient.Execute(restRequest);
                    if (!response.IsSuccessStatusCode)
                    {
                        UpdateWebsiteDownInfo(websiteItem);
                        _logger.LogInformation("Response: {@response}", response);
                        if (_websiteDownInfoList.SingleOrDefault(x => x.WebsiteUrl == websiteItem)?.DownCount >= _appSettings.DownCountLimit)
                        {
                            _telegramService.SendMessage($"{websiteItem} is down on {DateTime.UtcNow.ToString()}");
                        }
                    }
                    else
                    {
                        UpdateWebsiteDownInfo(websiteItem, true);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception on WebsiteDownTrack");
                }
            }
        }
        private void UpdateWebsiteDownInfo(string websiteUrl, bool reset = false)
        {
            if (reset)
            {
                var existingRecord = _websiteDownInfoList.SingleOrDefault(x => x.WebsiteUrl == websiteUrl);
                if (existingRecord != null)
                {
                    _websiteDownInfoList.Remove(existingRecord);
                }
            }
            else if (_websiteDownInfoList.Any(x => x.WebsiteUrl == websiteUrl))
            {
                var existingRecord = _websiteDownInfoList.Single(x => x.WebsiteUrl == websiteUrl);
                existingRecord.DownCount++;
            }
            else
            {
                var websiteDownInfo = new WebsiteDownInfo(websiteUrl);
                _websiteDownInfoList.Add(websiteDownInfo);
            }
        }
    }
}
