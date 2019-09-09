using InfoScreenPi.Entities;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using InfoScreenPi.ViewModels;

namespace InfoScreenPi.Hubs
{
    public class WebSocketHub : Hub<IWebSocketClient>
    {

        public async Task RefreshScreens()
        {
            await Clients.AllExcept(new [] {Context.ConnectionId}).RefreshScreens();
            //await Clients.All.SendAsync("Refresh");
        }

        public async Task BroadcastSlide(ScreenItemViewModel currentSlide)
        {
            await Clients.AllExcept(new [] {Context.ConnectionId}).BroadcastSlide(currentSlide);

        }
    }
}
