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
using System.Collections.Generic;
using StealNews.Model.Entities;
using StealNews.Model.Models.Service.Notification;
using StealNews.Core.NotificationSenders;

namespace StealNews.Core.Services.Implementation
{
    public class BackgroundNewsGenerator : BackgroundService
    {
        private readonly ILogger _logger = Logger.GetLogger(typeof(BackgroundNewsGenerator));
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly BackgroundWorkerConfiguration _workersConfiguration;

        public BackgroundNewsGenerator(IServiceScopeFactory serviceScopeFactory, IOptions<BackgroundWorkerConfiguration> config)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _workersConfiguration = config.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Start Executing BackgroundNewsGenerator - {DateTime.UtcNow} UTC");

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var utcNow = DateTime.UtcNow;

                    if (utcNow.Hour >= _workersConfiguration.TimeOfStartingWorkersHoursUtc && utcNow.Hour <= _workersConfiguration.TimeOfEndingWorkersHoursUtc)
                    {
                        try
                        {
                            _logger.LogInformation($"Start Generating news - {DateTime.UtcNow} UTC");

                            var newsGenerator = scope.ServiceProvider.GetRequiredService<INewsGenerator>();
                            var generatedNews = await newsGenerator.GenerateAsync();

                            _logger.LogInformation($"Count generated news: {generatedNews.Count()} - {utcNow} UTC");

                            var notificationSenders = scope.ServiceProvider.GetServices<INotificationSender>();
                            var newNewsNotifications = GetNewsNotifications(generatedNews);

                            var sendTasks = new List<Task>();
                            foreach (var sender in notificationSenders)
                            {
                                var sendTask = sender.SendAsync(newNewsNotifications);
                                sendTasks.Add(sendTask);
                            }

                            await Task.WhenAll(sendTasks);
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
            }

            _logger.LogInformation($"End Executing BackgroundNewsGenerator - {DateTime.UtcNow} UTC");
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
