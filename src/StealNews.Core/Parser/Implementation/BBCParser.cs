using AngleSharp.Html.Dom;
using StealNews.Core.Parser.Abstraction;
using StealNews.Core.Settings;
using StealNews.Model.Entities;
using StealNews.Model.Models.Parser;
using System;
using System.Linq;

namespace StealNews.Core.Parser.Implementation
{
    public class BBCParser : BaseNewsParser
    {
        protected override CommonInfo ParseCommonInfo(IHtmlDocument document)
        {
            var title = document.QuerySelector(".story-body__h1").TextContent;
            var paragraphes = document.QuerySelectorAll(".story-body__inner > p").Select(p => p.TextContent);
            var text = string.Join(Environment.NewLine, paragraphes);

            var countDescriptionSymbols = text.Length > ParserConstants.COUNT_SYMBOLS_FOR_DESCRIPTIONS ? ParserConstants.COUNT_SYMBOLS_FOR_DESCRIPTIONS : text.Length;
            var description = text.Substring(0, countDescriptionSymbols);
            var words = description.Split(" ", StringSplitOptions.RemoveEmptyEntries).ToList();
            words.RemoveAt(words.Count - 1);
            var parsedDescription = $"{string.Join(" ", words)}...";

            var dateSeconds = double.Parse(document.QuerySelector(".date--v2").GetAttribute("data-seconds"));
            var time = TimeSpan.FromSeconds(dateSeconds);
            var date = DateTime.MinValue.AddSeconds(dateSeconds);
            var createdDate = new DateTime(DateTime.UtcNow.Year, date.Month, date.Day, date.Hour, date.Minute, 0);

            var commonInfo = new CommonInfo()
            {
                Title = title,
                Text = text,
                Description = parsedDescription,
                CreatedDate = createdDate
            };

            return commonInfo;
        }

        protected override Category ParseCategories(IHtmlDocument document)
        {
            var mainCategory = document.QuerySelector(".secondary-navigation__title > span")?.TextContent;
            if (mainCategory == null)
            {
                mainCategory = document.QuerySelector(".index-title__container > a").TextContent;
            }

            var subCategories = document.QuerySelectorAll(".tags-list__tags > a").Select(sc => sc.TextContent);

            var category = new Category()
            {
                Title = mainCategory,
                SubCategories = subCategories
            };

            return category;
        }

        protected override Images ParseImages(IHtmlDocument document)
        {
            var baseSiteUrl = GetBaseSiteUrl(document);

            var allImages = document.QuerySelectorAll(".story-body__inner > figure img").Select(i => i.GetAttribute("src"));
            var mainImage = allImages.First();
            var additionalImages = allImages.Skip(1);

            var images = new Images()
            {
                MainImage = mainImage,
                AdditionalImages = additionalImages
            };

            return images;
        }

        protected override string ParseSourceLogo(IHtmlDocument document)
        {
            return document.QuerySelector("link[rel=apple-touch-icon-precomposed]").GetAttribute("href");
        }

        private string GetBaseSiteUrl(IHtmlDocument document)
        {
            var siteUrl = document.QuerySelector("meta[property='og:url']").GetAttribute("content");
            var siteUri = new Uri(siteUrl);
            return $"{siteUri.Scheme}://{siteUri.Host}";
        }
    }
}
