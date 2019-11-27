using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using StealNews.Core.SourceGenerator.Abstraction;
using StealNews.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace StealNews.Core.SourceGenerator.Implementation
{
    public class BeltaSourceGenerator : ISourceGenerator
    {
        public async Task<IEnumerable<string>> GenerateAsync(string siteTemplate, int count, int skip = 0)
        {
            if(siteTemplate == null)
            {
                throw new ArgumentNullException(nameof(siteTemplate));
            }
            if (count < 0)
            {
                throw new ArgumentException("Count can not be less 0", nameof(count));
            }
            if (skip < 0)
            {
                throw new ArgumentException("Skip can not be less 0", nameof(skip));
            }
 
            var sources = new List<string>();

            var siteTemplateUri = new Uri(siteTemplate);
            var baseSiteUrl = $"{siteTemplateUri.Scheme}://{siteTemplateUri.Host}";
            var baseUri = new Uri(baseSiteUrl);

            var hch = new HttpClientHandler() { Proxy = null, UseProxy = false };
            var httpClient = new HttpClient(hch);
            var parser = new HtmlParser();

            var countItemsOnPage = 30;
            var skipedPage = (int)Math.Floor((double)skip / countItemsOnPage);
            var skipeditems = skip - skipedPage * countItemsOnPage;
            var page = skipedPage + 1;

            List<string> hrefs = null;

            do
            {   
                var sourceUrl = string.Format(siteTemplate, page);
                var html = await HttpHelper.ReadAsync(sourceUrl, httpClient);

                var document = await parser.ParseDocumentAsync(html, CancellationToken.None);
                hrefs = document.QuerySelectorAll(".news_item.lenta_item > a").Select(el => el.GetAttribute("href")).ToList();

                for (int i = skipeditems; i < hrefs.Count; i++)
                {
                    if (sources.Count == count)
                        break;

                    var sourceUri = new Uri(baseUri, hrefs[i]);
                    sources.Add(sourceUri.AbsoluteUri);
                }

                skipeditems = 0;
                page++;
            }
            while (sources.Count < count && hrefs.Count() > 0);

            return sources;
        }
    }
}
