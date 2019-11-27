using FCM.Net;
using StealNews.Model.Models.Service.Notification;
using System.Threading.Tasks;

namespace StealNews.Core.Services.Abstraction
{
    public interface INotificationService
    {
        Task<PushResponse> SendAsync(Message message);
    }
}
