using AngleSharp.Html.Parser;
using StealNews.Core.SourceValidators.Abstraction;
using StealNews.Helpers.Common;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace StealNews.Core.SourceValidators.Implementation
{
    public class BeltaSourceValidator : ISourceValidator
    {
        public async Task<IEnumerable<string>> ValidateAsync(IEnumerable<string> sources)
        {
            if(sources == null)
            {
                throw new ArgumentNullException(nameof(sources));
            }

            var validatedSources = new List<string>();

            var hch = new HttpClientHandler() { Proxy = null, UseProxy = false };
            var httpClient = new HttpClient(hch);
            var parser = new HtmlParser();

            foreach (var source in sources)
            {
                var html = await HttpReader.ReadAsync(source, httpClient);
                var document = await parser.ParseDocumentAsync(html, CancellationToken.None);

                var haveContent = document.QuerySelector(".content_margin") != null;
                var haveMainImage = document.QuerySelector(".main_img") != null;

                if(haveContent && haveMainImage)
                {
                    validatedSources.Add(source);
                }
            }

            return validatedSources;
        }
    }
}
