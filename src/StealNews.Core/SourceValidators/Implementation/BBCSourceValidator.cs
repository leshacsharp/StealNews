using AngleSharp.Html.Parser;
using StealNews.Common.Helpers;
using StealNews.Core.SourceValidators.Abstraction;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace StealNews.Core.SourceValidators.Implementation
{
    public class BBCSourceValidator : ISourceValidator
    {
        public async Task<IEnumerable<string>> ValidateAsync(IEnumerable<string> sources)
        {
            if (sources == null)
            {
                throw new ArgumentNullException(nameof(sources));
            }

            var validatedSources = new List<string>();

            var hch = new HttpClientHandler() { Proxy = null, UseProxy = false };
            var httpClient = new HttpClient(hch);
            var parser = new HtmlParser();

            foreach (var source in sources)
            {
                var html = await HttpHelper.ReadAsync(source, httpClient);
                var document = await parser.ParseDocumentAsync(html);

                var haveImages = document.QuerySelectorAll(".story-body__inner > figure img").Length > 0;

                if(haveImages)
                {
                    validatedSources.Add(source);
                }
            }

            return validatedSources;
        }
    }
}
