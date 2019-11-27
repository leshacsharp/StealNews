using System.Net;

namespace StealNews.Model.Models.Service.Notification
{
    public class PushResponse
    {
        public PushResponse(HttpStatusCode statusCode, string reasonPhrase, string content)
        {
            StatusCode = statusCode;
            ReasonPhrase = reasonPhrase;
            Content = content;
        }

        public HttpStatusCode StatusCode { get; }
        public string ReasonPhrase { get; }
        public string Content { get; set; }
    }
}
