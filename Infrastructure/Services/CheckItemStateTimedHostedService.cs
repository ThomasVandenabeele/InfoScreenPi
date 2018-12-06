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

namespace InfoScreenPi.Infrastructure.Services
{
    public class CheckItemStateTimedHostedService : IHostedService, IDisposable
    {

        private readonly IServiceScopeFactory _scopeFactory;
        private IHubContext<WebSocketHub, IWebSocketHub> _hubContext;
        private readonly ILogger _logger;
        private Timer _timer;


        public CheckItemStateTimedHostedService(
                ILogger<RefreshRSSTimedHostedService> logger, 
                IHubContext<WebSocketHub, IWebSocketHub> hubContext,
                IServiceScopeFactory scopeFactory
            )
        {
            _hubContext = hubContext;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("CheckItemStateTimedHostedService is starting.");

            _timer = new Timer(DoCheckItemState, null, TimeSpan.Zero, TimeSpan.FromSeconds(15));
            //_timer = new Timer(DoRssRefresh, null, TimeSpan.Zero, TimeSpan.FromHours(1));

            return Task.CompletedTask;
        }

        private async void DoCheckItemState(object state)
        {
            _logger.LogInformation("CheckItemStateTimedHostedService is working.");

            using (var scope = _scopeFactory.CreateScope())
            {
                var itemScope = scope.ServiceProvider.GetRequiredService<IItemRepository>();
                var changed = itemScope.CheckItemState();
                
                //Enkel refresh indien items zijn gewijzigd               
                if(changed) _hubContext.Clients.All.RefreshScreens();
            }
            
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("CheckItemStateTimedHostedService is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}