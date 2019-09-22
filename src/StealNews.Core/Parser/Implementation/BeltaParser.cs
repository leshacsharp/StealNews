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

            var commonInfo = new CommonInfo()
            {
                Title = title,
                Text = articleText,
                Description = description,
            };

            return Task.FromResult(commonInfo);
        }

        protected override Task<Images> ParseImagesAsync(IHtmlDocument document)
        {
            var mainImage = document.QuerySelector(".main_img > img").GetAttribute("src");
            var additionalImages = document.QuerySelectorAll(".js-mediator-article > img").Select(im => im.GetAttribute("src"));

            var images = new Images()
            {
                MainImage = mainImage,
                AdditionalImages = additionalImages
            };

            return Task.FromResult(images);
        }

        protected override Task<Category> ParseCategoriesAsync(IHtmlDocument document)
        {
            var categoryTitle = document.QuerySelector(".content_margin > h1").TextContent;
            var subCategoriesTitles = document.QuerySelectorAll(".tag_item").Select(c => c.TextContent);

            var category = new Category()
            {
                Title = categoryTitle,
                SubCategories = subCategoriesTitles.Select(cat => new Category() { Title = cat })
            };

            return Task.FromResult(category);
        }
    }
}
