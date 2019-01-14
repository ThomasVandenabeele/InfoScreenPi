using System.Threading.Tasks;
using InfoScreenPi.Entities;
using Microsoft.AspNetCore.SignalR;

namespace InfoScreenPi.Hubs
{
    public interface IWebSocketClient
    {
        Task RefreshScreens();
        Task BroadcastSlide(Item currentSlide);
    }
}