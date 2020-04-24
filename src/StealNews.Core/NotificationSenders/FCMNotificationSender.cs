using Microsoft.Extensions.Logging;
using StealNews.Common.Logging;
using StealNews.Core.Services.Abstraction;
using StealNews.Model.Models.Service.Notification;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StealNews.Core.NotificationSenders
{
    public class FCMNotificationSender : INotificationSender
    {
        private readonly ILogger _logger = Logger.GetLogger(typeof(FCMNotificationSender));
        private readonly INotificationService _notificationService;

        public FCMNotificationSender(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task SendAsync(IEnumerable<NewsNotification> notifications)
        {
            foreach (var notification in notifications)
            {
                var message = new FCM.Net.Message()
                {
                    To = $"/topics/{notification.CategoryCode}",
                    Notification = new FCM.Net.Notification()
                    {
                        Title = "StealNews",
                        Body = $"{notification.CategoryTitle} = {notification.CountNews}",//JsonSerializer.Serialize(notification),
                        Color = "blue"
                    }
                };

                var response = await _notificationService.SendAsync(message);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    var warningMessage = $"FCM send {response.StatusCode} code. \nReason={response.ReasonPhrase} \nContent={response.Content}";
                    _logger.LogWarning(warningMessage);
                }
            }
        }
    }
}
