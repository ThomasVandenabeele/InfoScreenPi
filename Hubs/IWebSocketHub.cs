using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace InfoScreenPi.Hubs
{
    public interface IWebSocketHub
    {
        Task RefreshScreens();
    }
}