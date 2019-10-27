using System;
using System.Collections.Generic;
using System.Net.WebSockets;

namespace StealNews.Core.Services.Abstraction
{
    public interface IWebSocketService
    {
        void Add(WebSocket socket);
        WebSocket Remove(Guid id);
        Guid GetSocketId(WebSocket socket);
        WebSocket GetById(Guid id);
        IEnumerable<WebSocket> GetAll();
    }
}
