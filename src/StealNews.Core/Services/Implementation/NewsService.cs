using Microsoft.Extensions.Options;
using StealNews.Core.ComponentsFactory;
using StealNews.Core.Services.Abstraction;
using StealNews.Core.Settings;
using StealNews.DataProvider.Repositories.Abstraction;
using StealNews.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using StealNews.Model.Models.Service.News;

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
            //Order of adding important
            var generatedNews = new List<News>();

            foreach (var source in _sourceConfiguration.Sources)
            {
                var componentsFabric = ComponentsProvider.CreateComponentsFactory(source.SiteTitle);
                var sourceGenerator = componentsFabric.CreateSourceGenertor();
                var sourceValidator = componentsFabric.CreateSourceValidator();
                var htmlParser = componentsFabric.CreateNewsParser();

                var filterBySource = Builders<News>.Filter.Where(n => n.Source.SiteTitle == source.SiteTitle);
                var newsBySource = await _newsRepository.FindAsync(filterBySource);
                var lastNews = newsBySource.OrderByDescending(n => n.Id).FirstOrDefault();

                var partsOfNews = new List<PartOfNews>();
                IEnumerable<string> sourcesUrl = null;
                ICollection<News> newNewsBySource = null;
                var isLastNewsFinded = false;
                var isPageHaveLastNews = false;
                var skipNews = 0;

                do
                {
                    sourcesUrl = await sourceGenerator.GenerateAsync(source.SiteTemplate, _sourceConfiguration.CountGeneratedNewsFor1Time, skipNews);
                    sourcesUrl = await sourceValidator.ValidateAsync(sourcesUrl);
                    sourcesUrl = sourcesUrl.Reverse();

                    newNewsBySource = new List<News>();

                    foreach (var sourceUrl in sourcesUrl)
                    {
                        var news = await htmlParser.ParseAsync(sourceUrl);

                        if (news.Equals(lastNews) || lastNews == null)
                        {
                            isPageHaveLastNews = true;
                            isLastNewsFinded = true;
                        }

                        newNewsBySource.Add(news);
                    }

                    var partOfNews = new PartOfNews()
                    {
                        News = newNewsBySource,
                        IsPageHaveLastNews = isPageHaveLastNews
                    };

                    partsOfNews.Add(partOfNews);
                    skipNews += _sourceConfiguration.CountGeneratedNewsFor1Time;
                }
                while (!isLastNewsFinded && lastNews != null && sourcesUrl.Count() > 0);


                for (int i = partsOfNews.Count - 1; i >= 0; i--)
                {
                    var part = partsOfNews[i];

                    if (part.IsPageHaveLastNews)
                    {
                        isLastNewsFinded = false;

                        foreach (var news in part.News)
                        {
                            if (isLastNewsFinded)
                            {
                                generatedNews.Add(news);
                            }

                            if (news.Equals(lastNews) || lastNews == null)
                            {
                                isLastNewsFinded = true;
                            }
                        }
                    }
                    else
                    {
                        generatedNews.AddRange(part.News);
                    }
                }
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

            return await _newsRepository.FindAsync(filter, filterModel.Count, filterModel.Skip);
        }
    }
}