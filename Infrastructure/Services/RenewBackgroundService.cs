using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;
using InfoScreenPi.Entities;
using InfoScreenPi.Hubs;
using InfoScreenPi.ViewModels;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InfoScreenPi.Infrastructure.Services
{
    public class RenewBackgroundService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private IVolatileDataService _data;
        private IRSSService _rss;

        private const int CheckItemStateInterval = 15;
        private const int CheckRSSRenewCount = 60 * 60 / CheckItemStateInterval;
    
        public RenewBackgroundService(ILoggerFactory loggerFactory,
                    IServiceScopeFactory scopeFactory)
        {
            _logger = loggerFactory.CreateLogger<ScreenBackgroundService>();
            _scopeFactory = scopeFactory;
        }
    
    
        protected async override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Renew Background Service is starting.");
            var counter = 0;
            using (var scope = _scopeFactory.CreateScope())
            {
                _data = scope.ServiceProvider.GetRequiredService<IVolatileDataService>();
                _rss = scope.ServiceProvider.GetRequiredService<IRSSService>();
               
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        counter++;
                        _logger.LogInformation("Checking state of items.");
                        _data.CheckItemState();
                        
                        if (counter >= CheckRSSRenewCount)
                        {
                            _logger.LogInformation("Renewing RSSFeed items.");
                            var renewed = _rss.RenewActiveRssFeeds().Result;
                            counter = 0;
                        }
    
                        
                        await Task.Delay(TimeSpan.FromSeconds(CheckItemStateInterval));
                        
    
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"An unexpected error occurred in RenewBackgroundController.");
                    }
                }
            }
    
            _logger.LogInformation("Renew Background Service is stopping.");
    
        }
    
    }
}