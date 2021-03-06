﻿using AngleSharp.Html.Parser;
using StealNews.Common.Helpers;
using StealNews.Core.SourceValidators.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace StealNews.Core.SourceValidators.Implementation
{
    public class BeltaSourceValidator : ISourceValidator
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
                var document = await parser.ParseDocumentAsync(html, CancellationToken.None);

                var haveContent = document.QuerySelector(".content_margin") != null;
                var haveMainImage = document.QuerySelector(".main_img") != null;
                var paragraphes = document.QuerySelectorAll(".js-mediator-article > p").Select(p => p.TextContent);
                var haveText = paragraphes.Count() > 0 && !string.IsNullOrEmpty(string.Join("", paragraphes));

                if (haveContent && haveText && haveMainImage)
                {
                    validatedSources.Add(source);
                }
            }

            return validatedSources;
        }
    }
}
