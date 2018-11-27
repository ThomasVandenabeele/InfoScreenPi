using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace InfoScreenPi.Hubs
{
    public class WebSocketHub : Hub<IWebSocketHub>
    {

        public async Task RefreshScreens()
        {
            await Clients.All.RefreshScreens();
            //await Clients.All.SendAsync("Refresh");
        }
    }
}
