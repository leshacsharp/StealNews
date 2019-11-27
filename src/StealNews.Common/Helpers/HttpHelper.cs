using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace StealNews.Common.Helpers
{
    public static class HttpHelper
    {
        public static async Task<string> ReadAsync(string siteUrl)
        {
            if (siteUrl == null)
            {
                throw new ArgumentNullException(nameof(siteUrl));
            }

            HttpClientHandler hch = new HttpClientHandler();
            hch.Proxy = null;
            hch.UseProxy = false;
            var httpClient = new HttpClient(hch);

            return await httpClient.GetStringAsync(siteUrl);
        }

        public static async Task<string> ReadAsync(string siteUrl, HttpClient httpClient)
        {
            if (siteUrl == null)
            {
                throw new ArgumentNullException(nameof(siteUrl));
            }

            if (httpClient == null)
            {
                throw new ArgumentNullException(nameof(httpClient));
            }

            return await httpClient.GetStringAsync(siteUrl);
        }
    }
}
