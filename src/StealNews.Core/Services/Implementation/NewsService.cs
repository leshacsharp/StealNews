using Microsoft.Extensions.Options;
using StealNews.Common.Helpers;
using StealNews.Core.ComponentsFactory;
using StealNews.Core.Models;
using StealNews.Core.Services.Abstraction;
using StealNews.Core.Settings;
using StealNews.DataProvider.Repositories.Abstraction;
using StealNews.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                var lastNews = _newsRepository.Read(news => news.Source.SiteTitle == source.SiteTitle)
                                              .OrderByDescending(n => n.Id)
                                              .Take(1);

                IEnumerable<string> sourcesUrl = null;
                var isLastNewsFinded = false;

                do
                {
                    sourcesUrl = await sourceGenerator.GenerateAsync(source.SiteTemplate, _sourceConfiguration.CountGeneratedNewsFor1Time, skipNews);

                    foreach (var sourceUrl in sourcesUrl)
                    {
                        var news = await htmlParser.ParseAsync(sourceUrl);

                        if(news.Equals(lastNews))
                        {
                            isLastNewsFinded = true;
                            break;
                        }

                        newNewsBySource.Add(news);
                    }

                    generatedNews.AddRange(newNewsBySource);
                    skipNews += _sourceConfiguration.CountGeneratedNewsFor1Time;
                }
                while (!isLastNewsFinded && sourcesUrl.Count() > 0);
            }

            await _newsRepository.BulkInsertAsync(generatedNews);
            return generatedNews;
        }

        public IEnumerable<News> Find(NewsFindFilter filter)
        {
            if(filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            var predicate = PredicateBuilder.True<News>();

            if (filter.Categories != null)
            {
                predicate = predicate.And(news => filter.Categories.Contains(news.Category.Title) || filter.Categories.Any(cat => news.Category.SubCategories.Contains(cat)));
            }

            if (filter.Sources != null)
            {
                predicate = predicate.And(news => filter.Sources.Contains(news.Source.SiteTitle));
            }

            if (filter.KeyWord != null)
            {
                predicate = predicate.And(news => news.Title.Contains(filter.KeyWord) || news.Text.Contains(filter.KeyWord));
            }

            if (filter.From != null)
            {
                predicate = predicate.And(news => news.CreatedDate >= filter.From);
            }

            if (filter.To != null)
            {
                predicate = predicate.And(news => news.CreatedDate <= filter.To);
            }  

            if(filter.AfterId != null)
            {
                predicate = predicate.And(news => news.Id > filter.AfterId);
            }

            return _newsRepository.Read(predicate).ToList();
        }
    }
}
