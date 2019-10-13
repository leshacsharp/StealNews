using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StealNews.DataProvider.Repositories.Abstraction;
using StealNews.DataProvider.Settings;
using StealNews.Model.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using StealNews.Model.Dto;

namespace StealNews.DataProvider.Repositories.Implementation
{
    public class NewsRepository : BaseRepository<News>, INewsRepository
    {
        public NewsRepository(IOptions<DbSettings> connectionString)
            : base(connectionString.Value.ConnectionString)
        {

        }

        public IEnumerable<CategoryDto> GetCategories()
        {

            var grCategories = (from n in Collection.AsQueryable()
                                group n by n.Category.Title into grByCategory
                                select new
                                {
                                    Title = grByCategory.Key,
                                    Count = grByCategory.Count(),
                                    SubCategories = grByCategory.Select(n => n.Category.SubCategories)
                                }).ToList();

            return from c in grCategories
                   select new CategoryDto()
                   {
                       Title = c.Title,
                       Count = c.Count,
                       SubCategories = c.SubCategories.SelectMany(sc => sc).Distinct()
                   }; 
        }

        public IEnumerable<SourceDto> GetSources()
        {
            return (from n in Collection.AsQueryable()
                    group n by n.Source into grBySources
                    select new SourceDto()
                    {
                        SiteTitle = grBySources.Key.SiteTitle,
                        SiteUrl = grBySources.Key.SiteUrl,
                        SiteLogo = grBySources.Key.SiteLogo,
                        Count = grBySources.Count()
                    }).ToList();
        }

        public async Task BulkInsertAsync(IEnumerable<News> news)
        {
            await Collection.InsertManyAsync(news);
        } 
    }
}
