using StealNews.Core.Services.Abstraction;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Linq;

namespace StealNews.Core.Services.Implementation
{
    public class WebSocketService : IWebSocketService
    {
        private readonly ConcurrentDictionary<Guid, WebSocket> _sockets;
        public WebSocketService()
        {
            _sockets = new ConcurrentDictionary<Guid, WebSocket>();
        }

        public void Add(WebSocket socket)
        {
            if(socket == null)
            {
                throw new ArgumentNullException(nameof(socket));
            }

            var socketId = GetSocketId();
            _sockets.TryAdd(socketId, socket);
        }

        public WebSocket Remove(Guid id)
        {
            WebSocket removedSocket = null;
            _sockets.TryRemove(id, out removedSocket);

            return removedSocket;
        }

        public WebSocket GetById(Guid id)
        {
            return _sockets.FirstOrDefault(s => s.Key == id).Value;
        }

        public Guid GetSocketId(WebSocket socket)
        {
            if (socket == null)
            {
                throw new ArgumentNullException(nameof(socket));
            }

            return _sockets.FirstOrDefault(s => s.Value == socket).Key;
        }

        public IEnumerable<WebSocket> GetAll()
        {
            return _sockets.Values;
        }

        private Guid GetSocketId()
        {
            return Guid.NewGuid();
        }
    }
}
