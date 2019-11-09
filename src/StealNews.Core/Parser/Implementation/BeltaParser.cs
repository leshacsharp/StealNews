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
        protected override CommonInfo ParseCommonInfo(IHtmlDocument document)
        {
            var title = document.QuerySelector(".content_margin > h1").TextContent;
            var paragraps = document.QuerySelectorAll(".js-mediator-article > p").Select(p => p.TextContent);
            var text = string.Join(Environment.NewLine, paragraps);

            var countDescriptionSymbols = text.Length > ParserConstants.COUNT_SYMBOLS_FOR_DESCRIPTIONS ? ParserConstants.COUNT_SYMBOLS_FOR_DESCRIPTIONS : text.Length;
            var description = text.Substring(0, countDescriptionSymbols);
            var words = description.Split(" ", StringSplitOptions.RemoveEmptyEntries).ToList();
            words.RemoveAt(words.Count - 1);
            var parsedDescription = $"{string.Join(" ", words)}...";

            var dateString = document.QuerySelector(".date_full").TextContent;
            var date = ParseDate(dateString);

            var commonInfo = new CommonInfo()
            {
                Title = title,
                Text = text,
                Description = parsedDescription,
                CreatedDate = date
            };

            return commonInfo;
        }

        protected override Images ParseImages(IHtmlDocument document)
        {
            var baseSiteUrl = GetBaseSiteUrl(document);

            var mainImage = document.QuerySelector(".main_img > img").GetAttribute("src");
            var additionalImages = document.QuerySelectorAll(".js-mediator-article img").Select(im => $"{baseSiteUrl}{im.GetAttribute("src")}");

            var images = new Images()
            {
                MainImage = mainImage,
                AdditionalImages = additionalImages
            };

            return images;
        }

        protected override Category ParseCategories(IHtmlDocument document)
        {
            var mainCategory = document.QuerySelector(".content_margin > a").TextContent;
            var subCategories = document.QuerySelectorAll(".tag_item").Select(c => c.TextContent);

            var category = new Category()
            {            
                Title = mainCategory,
                SubCategories = subCategories
            };

            return category;
        }

        protected override string ParseSourceLogo(IHtmlDocument document)
        {
            var baseSiteUrl = GetBaseSiteUrl(document);
            var logoUrl = document.QuerySelector("link[rel=icon]").GetAttribute("href");
            var fullLogoUrl = $"{baseSiteUrl}{logoUrl}";

            return fullLogoUrl;
        }

        private string GetBaseSiteUrl(IHtmlDocument document)
        {
            var siteUrl = document.QuerySelector("meta[property='og:url']").GetAttribute("content");
            var siteUri = new Uri(siteUrl);
            return $"{siteUri.Scheme}://{siteUri.Host}";
        }

        private DateTime ParseDate(string dateTime)
        {
            var parsedDate = dateTime.Split(",");
            var date = parsedDate[0];
            var time = parsedDate[1];

            var partsOfDate = date.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            var day = int.Parse(partsOfDate[0]);
            var month = Common.Helpers.DateHepler.GetMonthByName(partsOfDate[1]);
            var year = int.Parse(partsOfDate[2]);

            var partsOfTime = time.Split(":", StringSplitOptions.RemoveEmptyEntries);
            var hours = int.Parse(partsOfTime[0]);
            var minutes = int.Parse(partsOfTime[1]);

            return new DateTime(year, month, day, hours, minutes, 0);
        }
    }
}
