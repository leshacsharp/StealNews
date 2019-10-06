using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using StealNews.Core.Services.Abstraction;
using StealNews.Core.Settings;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace StealNews.Core.Services.Implementation
{
    public class BackgroundNewsGenerator : BackgroundService
    {
        private readonly INewsService _newsService;
        private readonly BackgroundWorkersConfiguration _workersConfiguration;

        public BackgroundNewsGenerator(IServiceProvider provider)//INewsService newsService, IOptions<BackgroundWorkersConfiguration> workersConfiguration)  
        {
            //Todo: wait .net core updating for IHostedService to can give dependencies from ctor
            _newsService = provider.GetRequiredService<INewsService>();
            _workersConfiguration = provider.GetRequiredService<IOptions<BackgroundWorkersConfiguration>>().Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                var currentHours = DateTime.UtcNow.Hour;
                if (currentHours > _workersConfiguration.TimeOfEndingWorkersHours && currentHours < _workersConfiguration.TimeOfStartingWorkersHours)
                {
                    var utcNow = DateTime.UtcNow;
                    var dateOfStartingWorkers = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day + 1, _workersConfiguration.TimeOfStartingWorkersHours, 0, 0);
                    var timeOfWaitingMs = (dateOfStartingWorkers - utcNow).Milliseconds;

                    await Task.Delay(timeOfWaitingMs, stoppingToken);
                }

                await _newsService.GenerateNewsAsync();

                await Task.Delay(_workersConfiguration.BackgroundNewsGeneratorTimeOutSec * 100, stoppingToken);
            }
        }
    }
}
