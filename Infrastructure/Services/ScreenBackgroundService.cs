using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using InfoScreenPi.Entities;
using InfoScreenPi.Hubs;
using InfoScreenPi.Infrastructure.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class ScreenBackgroundService : BackgroundService
{
    private readonly ILogger _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private IHubContext<WebSocketHub, IWebSocketClient> _hubContext;

    private IEnumerable<Item> activeCustomItems;
    private IEnumerable<Item> activeRSSItems;
 

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
            var _dataService = scope.ServiceProvider.GetRequiredService<IDataService>();

            // Get first item
            GetItemLists(_dataService);
            Item currentItem = activeCustomItems.FirstOrDefault();
            Item screenItem = currentItem;
            Item currentRssItem = null;
        
            while (!cancellationToken.IsCancellationRequested)
            {
                
                try
                {
                    if (screenItem != null)
                    {
                        // Send item to clients via websockets
                        _logger.LogInformation($"Send item: {(screenItem.Title)} with counter {counter}.");
                        //SEND TO CLIENTS
                        await _hubContext.Clients.All.BroadcastSlide(screenItem);
                
                        await Task.Delay(TimeSpan.FromSeconds(screenItem.DisplayTime));    
                    }
                    else
                    {
                        await Task.Delay(TimeSpan.FromSeconds(3));
                    }
                    
                    counter++;
                    
                    // Obtain new item
                    Random r = new Random();
                    bool checkIfRSSFeedActive =_dataService.GetAll<RssFeed>().Any(rf => rf.Active);
                    bool checkIfRssItemsExist = activeRSSItems.ToList().Count > 0;
                    bool checkIfRssShow = r.NextBool(25);
                    // if(_dataService.GetAll<RssFeed>().Any(rf => rf.Active) 
                    //     && activeRSSItems.ToList().Count > 0 
                    //     && r.NextBool(20)){
                    
                    //_logger.LogInformation($"ShowRss item: {(checkIfRssShow)} with counter {counter}.");
                    
                    if(checkIfRSSFeedActive && checkIfRssItemsExist && checkIfRssShow){
                        // Toon RSS Item als er actieve RSS feeds zijn en met een (pseudo) kans van 25%
                        if(currentRssItem == null){
                            currentRssItem = activeRSSItems.First();
                        } else{
                            currentRssItem = GetNextRssItem(currentRssItem, _dataService);
                        } 
                        
                        screenItem = currentRssItem ?? screenItem;

                    }
                    else{
                        currentItem = GetNextItem(currentItem, _dataService);
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

    private void GetItemLists(IDataService _dataService)
    {
        activeCustomItems = _dataService.GetAllActiveCustomItems();
        activeRSSItems = _dataService.GetAllActiveRSSItems(); 
    }

    Item GetNextItem(Item currentItem, IDataService _dataService)
    {
        GetItemLists(_dataService);
        if(activeCustomItems.Count() == 1) return activeCustomItems.First();
        Item next = activeCustomItems.SkipWhile(i => !i.Id.Equals(currentItem.Id)).Skip(1).FirstOrDefault();
        if(next == null)
        {
            //RefreshItemLists(_dataService);
            next = activeCustomItems.FirstOrDefault();
        }
        return next;
        //return activeCustomItems.SkipWhile(i => i.Equals(currentItem)).Skip(1).FirstOrDefault();
    }
    Item GetNextRssItem(Item currentRssItem, IDataService _dataService)
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
    }

    async Task<int> DoSomethingAync()  
    {   
        // You need to add a reference to System.Net.Http to declare client.  
        HttpClient client = new HttpClient();  
    
        // GetStringAsync returns a Task<string>. That means that when you await the  
        // task you'll get a string (urlContents).  
        Task<string> getStringTask = client.GetStringAsync("https://msdn.microsoft.com");  
    
        // You can do work here that doesn't rely on the string from GetStringAsync.  
        //DoIndependentWork();  
    
        // The await operator suspends AccessTheWebAsync.  
        //  - AccessTheWebAsync can't continue until getStringTask is complete.  
        //  - Meanwhile, control returns to the caller of AccessTheWebAsync.  
        //  - Control resumes here when getStringTask is complete.   
        //  - The await operator then retrieves the string result from getStringTask.  
        string urlContents = await getStringTask;  
    
        // The return statement specifies an integer result.  
        // Any methods that are awaiting AccessTheWebAsync retrieve the length value.  
        return urlContents.Length;  
    }  

    
}

public static class ExtensionMethods
{
    public static bool NextBool(this Random r, int truePercentage = 50)
    {
        return r.NextDouble() < truePercentage / 100.0;
    }
}
