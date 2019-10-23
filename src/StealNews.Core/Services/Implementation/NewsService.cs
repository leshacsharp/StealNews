using LinqKit;
using MongoDB.Driver;
using StealNews.Core.Services.Abstraction;
using StealNews.DataProvider.Repositories.Abstraction;
using StealNews.Model.Entities;
using StealNews.Model.Models.Service.News;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StealNews.Core.Services.Implementation
{
    public class NewsService : INewsService
    {
        private readonly INewsRepository _newsRepository;

        public NewsService(INewsRepository newsRepository)
        {
            _newsRepository = newsRepository;
        }

        public IEnumerable<News> Find(NewsFindFilter filterModel)
        {
            if (filterModel == null)
            {
                throw new ArgumentNullException(nameof(filterModel));
            }

            if (filterModel.Count <= 0)
            {
                return Enumerable.Empty<News>();
            }

            var filter = PredicateBuilder.New<News>(n => true);

            if (filterModel.MainCategories != null)
            {
                filter = filter.And(n => filterModel.MainCategories.Contains(n.Category.Title));
            }

            if (filterModel.SubCategories != null)
            {
                filter = filter.And(n => filterModel.SubCategories.Any(c => n.Category.SubCategories.Contains(c)));
            }

            if (filterModel.Sources != null)
            {
                filter = filter.And(n => filterModel.Sources.Contains(n.Source.SiteTitle));
            }

            if (filterModel.KeyWord != null)
            {
                filter = filter.And(n => n.Title.Contains(filterModel.KeyWord) || n.Text.Contains(filterModel.KeyWord));
            }

            if (filterModel.From != null)
            {
                filter = filter.And(n => n.CreatedDate >= filterModel.From);
            }

            if (filterModel.To != null)
            {
                filter = filter.And(n => n.CreatedDate <= filterModel.To);
            }

            var news = _newsRepository.Read(filter)
                                      .OrderByDescending(n => n.CreatedDate)
                                      .Skip(filterModel.Skip)
                                      .Take(filterModel.Count)
                                      .ToList();
            return news;
        }
    }
}