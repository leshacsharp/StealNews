using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StealNews.Core.ComponentsFactory;
using StealNews.Core.InfoGenerator.Abstraction;
using StealNews.Core.Services.Abstraction;
using StealNews.Core.Settings;
using StealNews.DataProvider.Repositories.Abstraction;
using StealNews.Model.Entities;
using StealNews.Model.Models.Service.News;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StealNews.Core.Services.Implementation
{
    public class NewsGenerator : INewsGenerator
    {
        private readonly INewsRepository _newsRepository;
        private readonly SourceConfiguration _sourceConfiguration;
        private readonly IServiceProvider _serviceProvider;

        public NewsGenerator(IServiceProvider serviceProvider, INewsRepository newsRepository, IOptions<SourceConfiguration> sources)
        {
            _serviceProvider = serviceProvider;
            _newsRepository = newsRepository;
            _sourceConfiguration = sources.Value;
        }

        public async Task<IEnumerable<News>> GenerateAsync()
        {
            //Order of adding important
            var generatedNews = new List<News>();

            foreach (var source in _sourceConfiguration.Sources)
            {
                var componentsFabric = ComponentsProvider.CreateComponentsFactory(source.SiteTitle);
                var sourceGenerator = componentsFabric.CreateSourceGenertor();
                var sourceValidator = componentsFabric.CreateSourceValidator();
                var htmlParser = componentsFabric.CreateNewsParser();

                var newsBySource = _newsRepository.Read(n => n.Source.SiteTitle == source.SiteTitle);
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

            var infoGenerators = _serviceProvider.GetServices<IInfoGenerator>();
            var generatorTasks = new List<Task>();

            foreach (var generator in infoGenerators)
            {
                var task = generator.ProcessAsync(generatedNews);
                generatorTasks.Add(task);
            }

            await Task.WhenAll(generatorTasks);

            if (generatedNews.Count > 0)
            {
                await _newsRepository.BulkInsertAsync(generatedNews);
            }

            return generatedNews;
        }
    }
}
