using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using InfoScreenPi.Entities;
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


    protected async override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Screen Background Service is starting.");
        var counter = 0;
        using (var scope = _scopeFactory.CreateScope())
        {
            _data = scope.ServiceProvider.GetRequiredService<IVolatileDataService>();
            IEnumerable<Item> activeCustomItems = _data.GetAllActive().Where(i => !(i is RSSItem));
            IEnumerable<Item> activeRSSItems = _data.GetAllActive().Where(i => i is RSSItem);

            Item currentItem = activeCustomItems.FirstOrDefault();
            Item screenItem = currentItem;
            Item currentRssItem = null;

            while (!cancellationToken.IsCancellationRequested)
            {
              activeCustomItems = _data.GetAllActive().Where(i => !(i is RSSItem));
              activeRSSItems = _data.GetAllActive().Where(i => i is RSSItem);
                try
                {
                    if (screenItem != null)
                    {
                        // Send item to clients via websockets
                        _logger.LogInformation($"Send item: {(screenItem.Title)} with counter {counter}.");
                        //SEND TO CLIENTS
                        await _hubContext.Clients.All.BroadcastSlide(new ScreenItemViewModel(screenItem));

                        await Task.Delay(TimeSpan.FromSeconds(screenItem.DisplayTime));
                    }
                    else
                    {
                        await Task.Delay(TimeSpan.FromSeconds(3));
                    }

                    counter++;

                    // Obtain new item
                    Random r = new Random();
                    bool checkIfRSSFeedActive = _data.AnyRssFeedActive();
                    bool checkIfRssItemsExist = activeRSSItems.ToList().Count > 0;
                    bool checkIfRssShow = r.NextBool(25);

                    if(checkIfRSSFeedActive && checkIfRssItemsExist && checkIfRssShow){
                        // Toon RSS Item als er actieve RSS feeds zijn en met een (pseudo) kans van 25%
                        /*if(currentRssItem == null){
                            currentRssItem = activeRSSItems.First();
                        } else{
                            if (activeRSSItems.Count() == 1) currentRssItem = activeRSSItems.First();
                            else{
                            Item next = activeRSSItems.SkipWhile(i => !i.Id.Equals(currentRssItem.Id)).Skip(1).FirstOrDefault();
                            if(next == null) currentRssItem = activeRSSItems.FirstOrDefault();
                          }*/
                        currentRssItem = GetNextItem(currentRssItem, activeRSSItems);
                        //}
                        screenItem = currentRssItem ?? screenItem;
                    }
                    else{
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
        if(activeItems.Count() == 1) return activeItems.First();
        T next = null;
        if (currentItem != null) next = activeItems.SkipWhile(i => !i.Id.Equals(currentItem.Id)).Skip(1).FirstOrDefault();
        if(next == null) next = activeItems.FirstOrDefault();
        return next;
        //return activeCustomItems.SkipWhile(i => i.Equals(currentItem)).Skip(1).FirstOrDefault();
    }
    /*Item GetNextRssItem(Item currentRssItem, IDataService _dataService)
    {
        GetItemLists(_dataService);
        if (activeRSSItems.Count() == 1) return activeRSSItems.First();
        Item next = activeRSSItems.SkipWhile(i => !i.Id.Equals(currentRssItem.Id)).Skip(1).FirstOrDefault();
        if(next == null)
        {
            //RefreshItemLists(_dataService);
            next = activeRSSItems.FirstOrDefault();
        }
        return next;
        //return activeCustomItems.SkipWhile(i => i.Equals(currentItem)).Skip(1).FirstOrDefault();
    }*/

    /*public IEnumerable<IExpiring> GetAllCustomItems()
    {
        return _data.GetAllExpiring();
    }
    public IEnumerable<IExpiring> GetAllActiveCustomItems()
    {
        return GetAllCustomItems().Where(i => i.Active && !i.Archieved);
    }**/
}

public static class ExtensionMethods
{
    public static bool NextBool(this Random r, int truePercentage = 50)
    {
        return r.NextDouble() < truePercentage / 100.0;
    }
}

}
