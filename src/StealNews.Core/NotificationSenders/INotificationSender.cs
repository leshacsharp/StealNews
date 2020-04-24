using StealNews.Model.Models.Service.Notification;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StealNews.Core.NotificationSenders
{
    public interface INotificationSender
    {
        Task SendAsync(IEnumerable<NewsNotification> notifications);
    }
}
