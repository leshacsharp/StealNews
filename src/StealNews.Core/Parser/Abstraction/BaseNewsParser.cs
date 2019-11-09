using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using StealNews.Model.Models.Parser;
using StealNews.Model.Entities;
using System;
using System.Threading.Tasks;
using StealNews.Common.Helpers;

namespace StealNews.Core.Parser.Abstraction
{
    public abstract class BaseNewsParser : INewsParser
    {
        public async Task<News> ParseAsync(string source)
        {
            if(source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var uri = new Uri(source);
            var sourceInfo = new Source()
            {
                SiteTitle = uri.Host.Remove(0, 4),
                SiteUrl = $"{uri.Scheme}://{uri.Host}",
            };

            var html = await HttpHelper.ReadAsync(source);
            var htmlParser = new HtmlParser();
            var document = await htmlParser.ParseDocumentAsync(html);

            var commonInfo = ParseCommonInfo(document);
            var categories = ParseCategories(document);
            var images = ParseImages(document);
            var sourceLogo = ParseSourceLogo(document);

            sourceInfo.SiteLogo = sourceLogo;

            var newsItem = new News()
            {
                Title = commonInfo.Title,
                Url = source,
                Text = commonInfo.Text,
                Description = commonInfo.Description,
                CreatedDate = commonInfo.CreatedDate,
                Category = categories,
                Source = sourceInfo,
                MainImage = images.MainImage,
                Images = images.AdditionalImages
            };

            return newsItem;
        }

        protected abstract CommonInfo ParseCommonInfo(IHtmlDocument document);

        protected abstract Category ParseCategories(IHtmlDocument document);

        protected abstract Images ParseImages(IHtmlDocument document);

        protected abstract string ParseSourceLogo(IHtmlDocument document);
    }
}
