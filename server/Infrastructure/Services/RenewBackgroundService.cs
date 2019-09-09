using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
                            _logger.LogInformation("Renewing RSSFeed items and weather forecast.");
                            var renewed = _rss.RenewActiveRssFeeds().Result;

                            using (var client = new HttpClient())
                            {
                                try
                                {
                                    client.BaseAddress = new Uri("https://api.openweathermap.org");
                                    var city = _data.GetSettingByName("WeatherLocation");
                                    var response = await client.GetAsync($"/data/2.5/forecast?q={city}&appid=e9e81d9533487c8075cd2f47af1ef9ae&units=metric&lang=nl");
                                    response.EnsureSuccessStatusCode();

                                    var stringResult = await response.Content.ReadAsStringAsync();

                                    WeatherItem weather = _data.GetSingle<WeatherItem>();
                                    weather.Forecast = stringResult;
                                    _data.Edit(weather);
                                    _data.Commit();
                                    
                                }
                                catch (HttpRequestException httpRequestException)
                                {
                                    _logger.LogInformation($"Error getting weather from OpenWeather: {httpRequestException.Message}");
                                }
                            }
                            
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