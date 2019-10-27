using Microsoft.AspNetCore.Http;
using StealNews.Core.Managers.Abstraction;
using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace StealNews.WebAPI.Middlewares
{
    public class NewsWebSocketMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebSocketManager _webSocketManager;
        public NewsWebSocketMiddleware(RequestDelegate next, IWebSocketManager webSocketManager)
        {
            _next = next;
            _webSocketManager = webSocketManager;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if(context.WebSockets.IsWebSocketRequest)
            {
                var socket = await context.WebSockets.AcceptWebSocketAsync();
                _webSocketManager.OnConnected(socket);

                while (socket.State == WebSocketState.Open)
                {
                    var buffer = new byte[1024 * 4];
                    var arraySegment = new ArraySegment<byte>(buffer);

                    var result = await socket.ReceiveAsync(arraySegment, System.Threading.CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await _webSocketManager.OnDisconnectedAsync(socket);
                    }
                }
            }
        }
    }
}
