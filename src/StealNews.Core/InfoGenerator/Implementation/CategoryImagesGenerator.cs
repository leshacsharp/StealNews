using StealNews.Core.InfoGenerator.Abstraction;
using StealNews.Model.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using StealNews.Common.Helpers;
using Microsoft.Extensions.Options;
using StealNews.Core.Settings;

namespace StealNews.Core.InfoGenerator.Implementation
{
    public class CategoryImagesGenerator : IInfoGenerator
    {
        private readonly CategoryImagesGeneratorConfiguration _generatorConfiguration;
        public CategoryImagesGenerator(IOptions<InfoGeneratorsConfiguration> infoGeneratorsConfiguration)
        {
            _generatorConfiguration = infoGeneratorsConfiguration.Value.CategoryImagesGeneratorConfiguration;
        }

        public async Task ProcessAsync(IEnumerable<News> news)
        {
            if(news == null)
            {
                throw new ArgumentNullException(nameof(news));
            }

            var categories = news.Select(n => n.Category.Title).Distinct();
            var categoryImages = new Dictionary<string, string>();

            foreach (var category in categories)
            {
                var url = string.Format(_generatorConfiguration.ImagesApiUrlTemplate, _generatorConfiguration.AccessToken, category);
                var json = await HttpHelper.ReadAsync(url);
                var imageUrl = await JsonHelper.GetAsync<string>(_generatorConfiguration.ImageUrlPropertyName, json);

                categoryImages.Add(category, imageUrl);
            }

            foreach (var newsItem in news)
            {
                var categoryImage = categoryImages[newsItem.Category.Title];
                newsItem.Category.Image = categoryImage;
            }
        }
    }
}
