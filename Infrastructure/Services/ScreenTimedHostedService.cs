using System;
using System.Linq;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using InfoScreenPi.Entities;
using Microsoft.AspNetCore.SignalR;
using InfoScreenPi.Hubs;
using InfoScreenPi.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace InfoScreenPi.Infrastructure.Services
{
    public class ScreenTimedHostedService : IHostedService, IDisposable
    {

        private readonly IServiceScopeFactory _scopeFactory;
        private IHubContext<WebSocketHub, IWebSocketClient> _hubContext;
        private readonly ILogger _logger;
        private Timer _timer;

        private IEnumerable<Item> activeItems;
        private IEnumerable<Item> rssItems;

        private int currentItemId;

        public ScreenTimedHostedService(
                ILogger<RefreshRSSTimedHostedService> logger, 
                IHubContext<WebSocketHub, IWebSocketClient> hubContext,
                IServiceScopeFactory scopeFactory
            )
        {
            _hubContext = hubContext;
            _logger = logger;
            _scopeFactory = scopeFactory;
            
            RenewItemLists();

        }

        public void RenewItemLists()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var itemRepo = scope.ServiceProvider.GetRequiredService<IItemRepository>();
                activeItems = itemRepo.GetAll(i => i.Active);
                rssItems = itemRepo.GetAllActiveRSSItems();
                currentItemId = activeItems.First().Id;
            }
        }

        public Item GetCurrentItem()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var itemRepo = scope.ServiceProvider.GetRequiredService<IItemRepository>();
                return itemRepo.GetAll(i => i.Active).First(i => i.Id.Equals(currentItemId));
            }
        }

        public Item GetNextItem()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var itemRepo = scope.ServiceProvider.GetRequiredService<IItemRepository>();
                Item next = itemRepo.GetAll(i => i.Active).SkipWhile(i => i.Id.Equals(currentItemId)).Skip(1).FirstOrDefault();
                // if(next == null)
                // {
                //     RenewItemLists();
                //     next = GetCurrentItem();
                // }
                currentItemId = next.Id;
                return next;
            }
        }

        public int NextCurrentItemId(int id)
        {
            return activeItems.SkipWhile(i => i.Id.Equals(id)).Skip(1).FirstOrDefault().Id;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Screen service is starting.");

            var screenUpdaterTask = Task.Run(() => UpdateScreenTaskAsync()); 
            
            return Task.CompletedTask;
        }

        public async Task UpdateScreenTaskAsync()
        {
            Item currentItem = GetCurrentItem();
            while(true)
            {
                _logger.LogInformation("Update screen to new slide.");

                //SEND TO CLIENTS
                await _hubContext.Clients.All.BroadcastSlide(currentItem);
                
                Task.Delay(currentItem.DisplayTime*1000).Wait();

                currentItem = GetNextItem();
                //currentItemId = NextCurrentItemId(currentItemId);
            
            }
        }

        private async void DoRssRefresh(object state)
        {
            _logger.LogInformation("Screen service is working.");

            using (var scope = _scopeFactory.CreateScope())
            {
                var rssFeedRepo = scope.ServiceProvider.GetRequiredService<IRssFeedRepository>();
                var renewed = rssFeedRepo.RenewActiveRssFeeds().Result;
                //Enkel refresh indien rssfeeds zijn gewijzigd

                var settingsRepo = scope.ServiceProvider.GetRequiredService<ISettingRepository>();
                var dbChanged = settingsRepo.GetSettingByName("DBChanged") == "True";
                
                if(renewed || dbChanged)
                {
                    var _context = scope.ServiceProvider.GetRequiredService<InfoScreenContext>();
                    
                    Setting s = _context.Settings.ToList().First(setting => setting.SettingName == "DBChanged");
                    s.SettingValue = false.ToString();
                    EntityEntry dbEnt = _context.Entry(s);
                    dbEnt.State = EntityState.Modified;
            
                    //Item test = _context.Items.Include(i => i.Background).Include(i => i.RssFeed).First(i => i.Current == true);

                    _context.SaveChanges();
                    
                    await _hubContext.Clients.All.RefreshScreens();
                    //await _hubContext.Clients.All.BroadcastSlide("ikke", test);
                    //_hubContext.Clients.All.SendAsync("RefreshScreens")
                }
            }
            
            //_rssFeedRepository.RenewActiveRssFeeds();
            //_hubContext.Clients.All.RefreshScreens();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Screen service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}