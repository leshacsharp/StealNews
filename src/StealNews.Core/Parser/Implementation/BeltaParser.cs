using AngleSharp.Html.Dom;
using StealNews.Core.Parser.Abstraction;
using StealNews.Core.Parser.Models;
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

            var dateString = document.QuerySelector(".date_full").TextContent;
            var date = ParseDate(dateString);

            var commonInfo = new CommonInfo()
            {
                Title = title,
                Text = articleText,
                Description = description,
                CreatedDate = date
            };

            return Task.FromResult(commonInfo);
        }

        protected override Task<Images> ParseImagesAsync(IHtmlDocument document)
        {
            var siteUrl = document.QuerySelector("meta[property='og:url']").GetAttribute("content");
            var siteUri = new Uri(siteUrl);
            var siteHost = $"{siteUri.Scheme}://{siteUri.Host}";

            var mainImage = document.QuerySelector(".main_img > img").GetAttribute("src");
            var additionalImages = document.QuerySelectorAll(".js-mediator-article img").Select(im => $"{siteHost}{im.GetAttribute("src")}");

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
                SubCategories = subCategoriesTitles.Select(cat => new Category() { Title = cat })
            };

            return Task.FromResult(category);
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
