using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using InfoScreenPi.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class ScreenBackgroundService : BackgroundService
{
    private readonly ILogger _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private IHubContext<WebSocketHub, IWebSocketClient> _hubContext;

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
        using (var scope = _scopeFactory.CreateScope())
        {
            var itemRepo = scope.ServiceProvider.GetRequiredService<IItemRepository>();
            return itemRepo.GetAll(i => i.Active).First(i => i.Id.Equals(currentItemId));
        }

        // Get first item

        while (!cancellationToken.IsCancellationRequested)
        {
            
            try
            {
                //int lengte = await DoSomethingAync();
                //_logger.LogInformation($"LENGTE IS {(lengte)}.");
                
                // Send item to clients via websockets
                // Wait for Item.DisplayTime and obtain new item.
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error occurred in ScreenBackgroundController.");
            }
        }

        _logger.LogInformation("Screen Background Service is stopping.");
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