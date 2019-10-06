using Microsoft.Extensions.Options;
using StealNews.Core.ComponentsFactory;
using StealNews.Model.Models.Service;
using StealNews.Core.Services.Abstraction;
using StealNews.Core.Settings;
using StealNews.DataProvider.Repositories.Abstraction;
using StealNews.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace StealNews.Core.Services.Implementation
{
    public class NewsService : INewsService
    {
        private readonly INewsRepository _newsRepository;
        private readonly SourceConfiguration _sourceConfiguration;

        public NewsService(INewsRepository newsRepository, IOptions<SourceConfiguration> sources)
        {
            _newsRepository = newsRepository;
            _sourceConfiguration = sources.Value;
        }

        public async Task<IEnumerable<News>> GenerateNewsAsync()
        {
            var generatedNews = new List<News>();

            foreach (var source in _sourceConfiguration.Sources)
            {
                var componentsFabric = ComponentsProvider.CreateComponentsFactory(source.SiteTitle);
                var sourceGenerator = componentsFabric.CreateSourceGenertor();
                var htmlParser = componentsFabric.CreateNewsParser();

                var newNewsBySource = new List<News>();
                var skipNews = 0;

                var filterBySource = Builders<News>.Filter.Where(n => n.Source.SiteTitle == source.SiteTitle);
                var newsBySource = await _newsRepository.FindAsync(filterBySource);
                var lastNews = newsBySource.OrderByDescending(n => n.Id).FirstOrDefault();

                IEnumerable<string> sourcesUrl = null;
                var isLastNewsFinded = false;

                do
                {
                    sourcesUrl = await sourceGenerator.GenerateAsync(source.SiteTemplate, _sourceConfiguration.CountGeneratedNewsFor1Time, skipNews);

                    foreach (var sourceUrl in sourcesUrl)
                    {
                        var news = await htmlParser.ParseAsync(sourceUrl);

                        if (isLastNewsFinded)
                        {
                            newNewsBySource.Add(news);
                        }

                        if (news.Equals(lastNews) || lastNews == null)
                        {
                            isLastNewsFinded = true;
                        }
                    }

                    generatedNews.AddRange(newNewsBySource);
                    skipNews += _sourceConfiguration.CountGeneratedNewsFor1Time;
                }
                while (!isLastNewsFinded && lastNews != null && sourcesUrl.Count() > 0);
            }

            if (generatedNews.Count > 0)
            {
                await _newsRepository.BulkInsertAsync(generatedNews);
            }

            return generatedNews;
        }

        public async Task<IEnumerable<News>> FindAsync(NewsFindFilter filterModel)
        {
            if (filterModel == null)
            {
                throw new ArgumentNullException(nameof(filterModel));
            }

            if (filterModel.Count <= 0 || filterModel.Skip < 0)
            {
                return await Task.FromResult(Enumerable.Empty<News>());
            }

            var builder = Builders<News>.Filter;
            var filter = builder.Empty;

            if (filterModel.Categories != null)
            {
                filter = filter & builder.Where(n => filterModel.Categories.Contains(n.Category.Title) || filterModel.Categories.Any(c => n.Category.SubCategories.Contains(c)));
            }

            if (filterModel.Sources != null)
            {
                filter = filter & builder.Where(n => filterModel.Sources.Contains(n.Source.SiteTitle));
            }

            if (filterModel.KeyWord != null)
            {
                filter = filter & builder.Where(n => n.Title.Contains(filterModel.KeyWord) || n.Text.Contains(filterModel.KeyWord));
            }

            if (filterModel.From != null)
            {
                filter = filter & builder.Where(n => n.CreatedDate >= filterModel.From);
            }

            if (filterModel.To != null)
            {
                filter = filter & builder.Where(n => n.CreatedDate <= filterModel.To);
            }

            if (filterModel.AfterId != null)
            {
                filter = filter & builder.Where(n => n.Id > filterModel.AfterId);
            }

            return await _newsRepository.FindAsync(filter, filterModel.Count, filterModel.Skip);
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            var categories = await _newsRepository.GetCategoriesAsync();

            return categories.GroupBy(c => c.Title)
                             .Select(c => new Category()
                             {
                                 Title = c.Key,
                                 SubCategories = c.SelectMany(p => p.SubCategories)
                                                  .Distinct()
                             });
                                    
        }
    }
}

