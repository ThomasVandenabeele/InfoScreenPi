using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using InfoScreenPi.Entities;
using InfoScreenPi.Extensions;
using InfoScreenPi.Hubs;
using InfoScreenPi.Infrastructure.Services;
using InfoScreenPi.ViewModels;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;


namespace InfoScreenPi.Infrastructure.Services
{

    public class ScreenBackgroundService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private IHubContext<WebSocketHub, IWebSocketClient> _hubContext;
        private IVolatileDataService _data;


        public ScreenBackgroundService(ILoggerFactory loggerFactory,
            IHubContext<WebSocketHub, IWebSocketClient> hubContext,
            IServiceScopeFactory scopeFactory)
        {
            _logger = loggerFactory.CreateLogger<ScreenBackgroundService>();
            _hubContext = hubContext;
            _scopeFactory = scopeFactory;
        }


        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Screen Background Service is starting.");
            using (var scope = _scopeFactory.CreateScope())
            {
                _data = scope.ServiceProvider.GetRequiredService<IVolatileDataService>();
                IEnumerable<Item> activeCustomItems =
                    _data.GetAllActive().Where(i => !(i is RSSItem || i is ClockItem || i is WeatherItem));
                IEnumerable<Item> activeRSSItems = _data.GetAllActive().Where(i => i is RSSItem);
                
                activeCustomItems.ToList().Shuffle();
                activeRSSItems.ToList().Shuffle();
                
                Item currentItem = activeCustomItems.FirstOrDefault();
                Item screenItem = currentItem;
                Item currentRssItem = null;

                bool showClock = true;

                while (!cancellationToken.IsCancellationRequested)
                {
                    activeCustomItems = _data.GetAllActive()
                        .Where(i => !(i is RSSItem || i is ClockItem || i is WeatherItem));
                    activeRSSItems = _data.GetAllActive().Where(i => i is RSSItem);
                    try
                    {
                        if (screenItem != null)
                        {
                            // Send item to clients via websockets
                            _logger.LogInformation($"Send item: {(screenItem.Title)}.");
                            //SEND TO CLIENTS
                            await _hubContext.Clients.All.BroadcastSlide(new ScreenItemViewModel(screenItem));

                            await Task.Delay(TimeSpan.FromSeconds(screenItem.DisplayTime));
                        }
                        else
                        {
                            await Task.Delay(TimeSpan.FromSeconds(3));
                        }

                        // Obtain new item
                        Random r = new Random();
                        ClockItem clock = _data.GetSingle<ClockItem>();
                        WeatherItem weather = _data.GetSingle<WeatherItem>();
//                    bool checkIfRSSFeedActive = _data.AnyRssFeedActive();
//                    bool checkIfRssItemsExist = activeRSSItems.ToList().Count > 0;
//                    bool checkIfRssShow = r.NextBool(25);

                        if (_data.AnyRssFeedActive() && activeRSSItems.ToList().Count > 0 && r.NextBool(25))
                        {
                            // Toon RSS Item als er actieve RSS feeds zijn en met een (pseudo) kans van 25%
                            currentRssItem = GetNextItem(currentRssItem, activeRSSItems);
                            screenItem = currentRssItem ?? screenItem;
                        }
                        else if (!(screenItem is ClockItem || screenItem is WeatherItem) && r.NextBool(35) &&
                                 ((showClock && clock.Active) || (!showClock && weather.Active)))
                        {
                            screenItem = showClock ? (Item) clock : weather;
                            showClock = !showClock;
                        }
                        else
                        {
                            currentItem = GetNextItem(currentItem, activeCustomItems);
                            screenItem = currentItem ?? screenItem;
                        }

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"An unexpected error occurred in ScreenBackgroundController.");
                    }
                }
            }

            _logger.LogInformation("Screen Background Service is stopping.");

        }

        T GetNextItem<T>(T currentItem, IEnumerable<T> activeItems) where T : Item
        {
            if (activeItems.Count() == 1) return activeItems.First();
            T next = null;
            if (currentItem != null)
                next = activeItems.SkipWhile(i => !i.Id.Equals(currentItem.Id)).Skip(1).FirstOrDefault();
            if (next == null) next = activeItems.FirstOrDefault();
            return next;
        }
    }

    public static class ExtensionMethods
    {
        public static bool NextBool(this Random r, int truePercentage = 50)
        {
            return r.NextDouble() < truePercentage / 100.0;
        }
        
        private static Random rng = new Random();  

        public static void Shuffle<T>(this IList<T> list)  
        {  
            int n = list.Count;  
            while (n > 1) {  
                n--;  
                int k = rng.Next(n + 1);  
                T value = list[k];  
                list[k] = list[n];  
                list[n] = value;  
            }  
        }
        
    }

    
}
