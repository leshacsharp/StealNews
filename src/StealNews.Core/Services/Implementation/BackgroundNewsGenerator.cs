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

namespace StealNews.Core.Services.Implementation
{
    public class BackgroundNewsGenerator : BackgroundService
    {
        private readonly ILogger _logger = Logger.GetLogger(typeof(BackgroundNewsGenerator));
        private readonly INewsGenerator _newsGenerator;
        private readonly BackgroundWorkerConfiguration _workersConfiguration;

        public BackgroundNewsGenerator(IServiceProvider provider)//INewsGenerator newsGenerator, IOptions<BackgroundWorkersConfiguration> workersConfiguration)  
        {
            //Todo: wait .net core updating for IHostedService to can give dependencies from ctor
            _newsGenerator = provider.GetRequiredService<INewsGenerator>();
            _workersConfiguration = provider.GetRequiredService<IOptions<BackgroundWorkerConfiguration>>().Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Start Executing BackgroundNewsGenerator - {DateTime.UtcNow} UTC");

            while (!stoppingToken.IsCancellationRequested)
            {
                var utcNow = DateTime.UtcNow;

                if (utcNow.Hour >= _workersConfiguration.TimeOfStartingWorkersHoursUtc && utcNow.Hour <= _workersConfiguration.TimeOfEndingWorkersHoursUtc)
                {
                    var generatedNews = await _newsGenerator.GenerateAsync();

                    _logger.LogInformation($"Count generated news: {generatedNews.Count()} - {utcNow} UTC");

                    await Task.Delay(_workersConfiguration.BackgroundNewsGeneratorTimeOutSec * 1000, stoppingToken);
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
    }
}
