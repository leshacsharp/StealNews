using System.Net.WebSockets;
using System.Threading.Tasks;

namespace StealNews.Core.Managers.Abstraction
{
    public interface IWebSocketManager
    {
        void OnConnected(WebSocket socket);
        Task OnDisconnectedAsync(WebSocket socket);
        Task SendAllAsync(string message);
    }
}
