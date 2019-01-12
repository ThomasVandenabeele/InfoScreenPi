using System;
using System.Linq;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using InfoScreenPi.Entities;
using Microsoft.AspNetCore.SignalR;
using InfoScreenPi.Hubs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;

namespace InfoScreenPi.Infrastructure.Services
{
    public class RefreshRSSTimedHostedService : IHostedService, IDisposable
    {

        private readonly IServiceScopeFactory _scopeFactory;
        private IHubContext<WebSocketHub, IWebSocketClient> _hubContext;
        private readonly ILogger _logger;
        private Timer _timer;


        public RefreshRSSTimedHostedService(
                ILogger<RefreshRSSTimedHostedService> logger, 
                IHubContext<WebSocketHub, IWebSocketClient> hubContext,
                IServiceScopeFactory scopeFactory
            )
        {
            _hubContext = hubContext;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("RSS refresh service is starting.");

            //_timer = new Timer(DoRssRefresh, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
            _timer = new Timer(DoRssRefresh, null, TimeSpan.Zero, TimeSpan.FromHours(1));

            return Task.CompletedTask;
        }

        private async void DoRssRefresh(object state)
        {
            _logger.LogInformation("RSS refresh service is working.");

            using (var scope = _scopeFactory.CreateScope())
            {
                var rssFeedRepo = scope.ServiceProvider.GetRequiredService<IRSSService>();
                var renewed = rssFeedRepo.RenewActiveRssFeeds().Result;
                //Enkel refresh indien rssfeeds zijn gewijzigd

                var dataService = scope.ServiceProvider.GetRequiredService<IDataService>();
                var dbChanged = dataService.GetSettingByName("DBChanged") == "True";

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
            _logger.LogInformation("RSS refresh service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
