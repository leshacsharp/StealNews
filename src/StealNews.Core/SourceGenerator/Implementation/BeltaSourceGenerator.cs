using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using StealNews.Core.Settings;
using StealNews.Core.SourceGenerator.Abstraction;
using StealNews.Helpers.Common;
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

            var hch = new HttpClientHandler() { Proxy = null, UseProxy = false };
            var httpClient = new HttpClient(hch);
            var parser = new HtmlParser();

            var countItemsOnPage = 30;
            var skipedPage = skip < countItemsOnPage ? 1 : (int)Math.Floor((double)skip / countItemsOnPage);
            var skipeditems = skip - skipedPage * countItemsOnPage;
            var page = skipedPage + 1;

            IHtmlCollection<IElement> htmlElements = null;

            do
            {   
                var sourceUrl = string.Format(siteTemplate, page);
                var html = await HttpReader.ReadAsync(sourceUrl, httpClient);

                var document = await parser.ParseDocumentAsync(html, CancellationToken.None);
                htmlElements = document.QuerySelectorAll(".news_item.lenta_item > a");

                for (int i = skipeditems + 1; i < htmlElements.Count(); i++)
                {
                    var sourceHref = htmlElements[i].GetAttribute("href");

                    if(sources.Count < count)
                    {
                        sources.Add(sourceHref);
                    }
                }

                skipeditems = 0;
                page++;
            }
            while (sources.Count < count && htmlElements.Count() > 0);

            return sources;
        }
    }
}
