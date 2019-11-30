using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using StealNews.Core.Services.Abstraction;
using StealNews.Core.Settings;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using StealNews.Common.Logging;
using Microsoft.Extensions.Logging;
using StealNews.Core.Managers.Abstraction;
using System.Collections.Generic;
using StealNews.Model.Entities;
using System.Text;
using StealNews.Model.Models.Service.Notification;
using System.Text.Json;

namespace StealNews.Core.Services.Implementation
{
    public class BackgroundNewsGenerator : BackgroundService
    {
        private readonly ILogger _logger = Logger.GetLogger(typeof(BackgroundNewsGenerator));
        private readonly INewsGenerator _newsGenerator;
        private readonly IWebSocketManager _webSocketManager;
        private readonly INotificationService _notificationService;
        private readonly BackgroundWorkerConfiguration _workersConfiguration;

        public BackgroundNewsGenerator(IServiceScopeFactory serviceScopeFactory)
        {
            //IServiceScopeFactory needed because we can't to inject scoped services in constructor in singletone(BackgroundNewsGenerator)
            using (var scope = serviceScopeFactory.CreateScope())
            {
                _newsGenerator = scope.ServiceProvider.GetRequiredService<INewsGenerator>();
                _webSocketManager = scope.ServiceProvider.GetRequiredService<IWebSocketManager>();
                _notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                _workersConfiguration = scope.ServiceProvider.GetRequiredService<IOptions<BackgroundWorkerConfiguration>>().Value;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Start Executing BackgroundNewsGenerator - {DateTime.UtcNow} UTC");

            while (!stoppingToken.IsCancellationRequested)
            {
                var utcNow = DateTime.UtcNow;

                if (utcNow.Hour >= _workersConfiguration.TimeOfStartingWorkersHoursUtc && utcNow.Hour <= _workersConfiguration.TimeOfEndingWorkersHoursUtc)
                {
                    try
                    {
                        _logger.LogInformation($"Start Generating news - {DateTime.UtcNow} UTC");

                        var generatedNews = await _newsGenerator.GenerateAsync();

                        _logger.LogInformation($"Count generated news: {generatedNews.Count()} - {utcNow} UTC");

                        var newNewsNotifications = GetNewsNotifications(generatedNews);
                        await SendNotificationsByFCMAsync(newNewsNotifications);
                        await SendNotificationsBySocketAsync(newNewsNotifications);

                        await Task.Delay(_workersConfiguration.BackgroundNewsGeneratorTimeOutSec * 1000, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Can't generate News");
                    }
                }
                else
                {
                    var timeOfWaitingMs = 0;

                    if (utcNow.Hour < _workersConfiguration.TimeOfStartingWorkersHoursUtc)
                    {
                        var timeOfStarting = new TimeSpan(_workersConfiguration.TimeOfStartingWorkersHoursUtc, 0, 0);
                        var currentTime = new TimeSpan(utcNow.Hour, utcNow.Minute, utcNow.Second);
                        timeOfWaitingMs = (int)(timeOfStarting - currentTime).TotalMilliseconds;
                    }
                    else
                    {
                        var dateOfStartingWorkers = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day + 1, _workersConfiguration.TimeOfStartingWorkersHoursUtc, 0, 0);
                        timeOfWaitingMs = (int)(dateOfStartingWorkers - utcNow).TotalMilliseconds;
                    }

                    _logger.LogInformation($"Time of waiting in hours:{(double)timeOfWaitingMs / 1000 / 60 / 60} Time of waiting in ms:{timeOfWaitingMs} - {utcNow} UTC");

                    await Task.Delay(timeOfWaitingMs, stoppingToken);
                }
            }

            _logger.LogInformation($"End Executing BackgroundNewsGenerator - {DateTime.UtcNow} UTC");
        }

        private async Task SendNotificationsByFCMAsync(IEnumerable<NewsNotification> notifications)
        {
            foreach (var notification in notifications)
            {
                var message = new FCM.Net.Message()
                {
                    To = $"/topics/{notification.CategoryCode}",
                    Notification = new FCM.Net.Notification()
                    {
                        Title = "StealNews - apperead new news",
                        Body = JsonSerializer.Serialize(notification)
                    }
                };

                var response = await _notificationService.SendAsync(message);
                if(response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    var warningMessage = $"FCM send {response.StatusCode} code. \nReason={response.ReasonPhrase} \nContent={response.Content}";
                    _logger.LogWarning(warningMessage);
                }
            }
        }

        private async Task SendNotificationsBySocketAsync(IEnumerable<NewsNotification> notifications)
        {
            var builder = new StringBuilder();
            foreach (var notification in notifications)
            {
                builder.Append($"{notification.CategoryTitle}={notification.CountNews};");
            }
            var generatedNewsInfo = builder.ToString();

            await _webSocketManager.SendAllAsync(generatedNewsInfo);
        }

        private IEnumerable<NewsNotification> GetNewsNotifications(IEnumerable<News> news)
        {
            var grNews = from n in news
                         group n by n.Category.Title into grNewsByCategory
                         select new NewsNotification()
                         {
                             CategoryTitle = grNewsByCategory.Key,
                             CategoryImage = news.First(n => n.Category.Title == grNewsByCategory.Key).Category.Image,
                             CountNews = grNewsByCategory.Count()
                         };

            return grNews;
        }
    }
}
