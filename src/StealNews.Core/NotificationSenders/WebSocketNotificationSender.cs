using StealNews.Core.Managers.Abstraction;
using StealNews.Model.Models.Service.Notification;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StealNews.Core.NotificationSenders
{
    public class WebSocketNotificationSender : INotificationSender
    {
        private readonly IWebSocketManager _webSocketManager;

        public WebSocketNotificationSender(IWebSocketManager webSocketManager)
        {
            _webSocketManager = webSocketManager;
        }

        public async Task SendAsync(IEnumerable<NewsNotification> notifications)
        {
            var builder = new StringBuilder();
            foreach (var notification in notifications)
            {
                builder.Append($"{notification.CategoryTitle}={notification.CountNews};");
            }
            var generatedNewsInfo = builder.ToString();

            await _webSocketManager.SendAllAsync(generatedNewsInfo);
        }
    }
}
