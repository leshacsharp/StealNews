using StealNews.Common.Helpers;
using StealNews.Core.SourceGenerator.Abstraction;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using AngleSharp.Html.Parser;

namespace StealNews.Core.SourceGenerator.Implementation
{
    public class BBCSourceGenerator : ISourceGenerator
    {
        public async Task<IEnumerable<string>> GenerateAsync(string source, int count, int skip = 0)
        {
            if(source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (count < 0)
            {
                throw new ArgumentException("Count can not be less 0", nameof(count));
            }
            if (skip < 0)
            {
                throw new ArgumentException("Skip can not be less 0", nameof(skip));
            }

            var parser = new HtmlParser();
            var html = await HttpHelper.ReadAsync(source);
            var document = parser.ParseDocument(html);

            var siteTemplateUri = new Uri(source);
            var baseSiteUrl = $"{siteTemplateUri.Scheme}://{siteTemplateUri.Host}";

            var allSources = document.QuerySelectorAll(".nw-c-most-read__items .gs-c-promo-heading")
                                     .Select(p => $"{baseSiteUrl}{p.GetAttribute("href")}");

            return allSources.Skip(skip).Take(count);
        }
    }
}
