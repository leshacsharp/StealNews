﻿using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using StealNews.Core.Parser.Models;
using StealNews.Helpers.Common;
using StealNews.Model.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

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
                SiteUrl = $"{uri.Scheme}://{uri.Host}"
            };


            var html = await HttpReader.ReadAsync(source);
            var htmlParser = new HtmlParser();
            var document = await htmlParser.ParseDocumentAsync(html);

            var commonInfoTask = ParseCommonInfoAsync(document);
            var categoriesTask = ParseCategoriesAsync(document);
            var imagesTask = ParseImagesAsync(document);

            await Task.WhenAll(commonInfoTask, categoriesTask, imagesTask);

            var newsItem = new News()
            {
                Title = commonInfoTask.Result.Title,
                Text = commonInfoTask.Result.Text,
                Description = commonInfoTask.Result.Description,
                CreatedDate = commonInfoTask.Result.CreatedDate,
                Category = categoriesTask.Result,
                Source = sourceInfo,
                MainImage = imagesTask.Result.MainImage,
                Images = imagesTask.Result.AdditionalImages
            };

            return newsItem;
        }

        protected abstract Task<CommonInfo> ParseCommonInfoAsync(IHtmlDocument document);

        protected abstract Task<Category> ParseCategoriesAsync(IHtmlDocument document);

        protected abstract Task<Images> ParseImagesAsync(IHtmlDocument document);
    }
}