using StealNews.Core.Managers.Abstraction;
using StealNews.Core.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StealNews.Core.Managers.Implementation
{
    public class WebSocketManager : IWebSocketManager
    {
        private readonly IWebSocketService _webSocketService;

        public WebSocketManager(IWebSocketService webSocketService)
        {
            _webSocketService = webSocketService;
        }

        public void OnConnected(WebSocket socket)
        {
            _webSocketService.Add(socket);
        }

        public async Task OnDisconnectedAsync(WebSocket socket)
        {
            var id = _webSocketService.GetSocketId(socket);
            var removedSocket = _webSocketService.Remove(id);
            await removedSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
        }

        public async Task SendAllAsync(string message)
        {
            var sockets = _webSocketService.GetAll();
            var sendTasks = new List<Task>();

            var buffer = Encoding.Default.GetBytes(message);
            var arraySegment = new ArraySegment<byte>(buffer);

            foreach (var socket in sockets)
            {
                var sendTask = socket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
                sendTasks.Add(sendTask);
            }

            await Task.WhenAll(sendTasks);
        }
    }
}
