using FCM.Net;
using Microsoft.Extensions.Options;
using StealNews.Core.Services.Abstraction;
using StealNews.Core.Settings;
using StealNews.Model.Models.Service.Notification;
using System;
using System.Threading.Tasks;

namespace StealNews.Core.Services.Implementation
{
    public class FCMNotificationService : INotificationService
    {
        private readonly NotificationManagerConfiguration _notificationConfiguration;
        public FCMNotificationService(IOptions<NotificationManagerConfiguration> notificationConfiguration)
        {
            _notificationConfiguration = notificationConfiguration.Value;
        }

        public async Task<PushResponse> SendAsync(Message message)
        {
            if(message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            using (var sender = new Sender(_notificationConfiguration.ServerAccessToken))
            {
                var response = await sender.SendAsync(message);

                var pushResponse = new PushResponse(response.StatusCode, response.ReasonPhrase, response.MessageResponse.ResponseContent);
                return pushResponse;
            }
        }
    }
}
