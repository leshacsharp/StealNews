using AngleSharp.Html.Dom;
using StealNews.Core.Parser.Abstraction;
using StealNews.Model.Models.Parser;
using StealNews.Core.Settings;
using StealNews.Model.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StealNews.Core.Parser.Implementation
{
    public class BeltaParser : BaseNewsParser
    {
        protected override Task<CommonInfo> ParseCommonInfoAsync(IHtmlDocument document)
        {
            var title = document.QuerySelector(".content_margin > h1").TextContent;
            var articleParagraps = document.QuerySelectorAll(".js-mediator-article > p").Select(p => p.TextContent);
            var articleText = string.Join(Environment.NewLine, articleParagraps);

            var description = articleText.Substring(0, ParserConstants.COUNT_SYMBOLS_FOR_DESCRIPTIONS);
            var words = description.Split(" ", StringSplitOptions.RemoveEmptyEntries).ToList();
            words.RemoveAt(words.Count - 1);
            var parsedDescription = $"{string.Join(" ", words)}...";

            var dateString = document.QuerySelector(".date_full").TextContent;
            var date = ParseDate(dateString);

            var commonInfo = new CommonInfo()
            {
                Title = title,
                Text = articleText,
                Description = parsedDescription,
                CreatedDate = date
            };

            return Task.FromResult(commonInfo);
        }

        protected override Task<Images> ParseImagesAsync(IHtmlDocument document)
        {
            var baseSiteUrl = GetBaseSiteUrl(document);

            var mainImage = document.QuerySelector(".main_img > img").GetAttribute("src");
            var additionalImages = document.QuerySelectorAll(".js-mediator-article img").Select(im => $"{baseSiteUrl}{im.GetAttribute("src")}");

            var images = new Images()
            {
                MainImage = mainImage,
                AdditionalImages = additionalImages
            };

            return Task.FromResult(images);
        }

        protected override Task<Category> ParseCategoriesAsync(IHtmlDocument document)
        {
            var categoryTitle = document.QuerySelector(".content_margin > a").TextContent;
            var subCategoriesTitles = document.QuerySelectorAll(".tag_item").Select(c => c.TextContent);

            var category = new Category()
            {
                Title = categoryTitle,
                SubCategories = subCategoriesTitles
            };

            return Task.FromResult(category);
        }

        protected override Task<string> ParseSourceLogoAsync(IHtmlDocument document)
        {
            var baseSiteUrl = GetBaseSiteUrl(document);
            var logoUrl = document.QuerySelector("link[rel=icon]").GetAttribute("href");
            var fullLogoUrl = $"{baseSiteUrl}{logoUrl}";

            return Task.FromResult(fullLogoUrl);
        }

        private string GetBaseSiteUrl(IHtmlDocument document)
        {
            var siteUrl = document.QuerySelector("meta[property='og:url']").GetAttribute("content");
            var siteUri = new Uri(siteUrl);
            return $"{siteUri.Scheme}://{siteUri.Host}";
        }

        private DateTime ParseDate(string date)
        {
            var parsedDate = date.Split(",")[0];
            var partsOfDate = parsedDate.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            var day = Convert.ToInt32(partsOfDate[0]);
            var month = Common.Helpers.DateHepler.GetMonthByName(partsOfDate[1]);
            var year = Convert.ToInt32(partsOfDate[2]);

            return new DateTime(year, month, day);
        }
    }
}
