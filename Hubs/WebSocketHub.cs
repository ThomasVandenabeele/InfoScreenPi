using InfoScreenPi.Entities;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace InfoScreenPi.Hubs
{
    public class WebSocketHub : Hub<IWebSocketClient>
    {

        public async Task RefreshScreens()
        {
            await Clients.AllExcept(new [] {Context.ConnectionId}).RefreshScreens();
            //await Clients.All.SendAsync("Refresh");
        }

        public async Task BroadcastSlide(Item currentSlide)
        {
            await Clients.AllExcept(new [] {Context.ConnectionId}).BroadcastSlide(currentSlide);

        }
    }
}
