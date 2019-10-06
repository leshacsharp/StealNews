using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using StealNews.DataProvider.Repositories.Abstraction;
using StealNews.DataProvider.Settings;
using StealNews.Model.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StealNews.DataProvider.Repositories.Implementation
{
    public class NewsRepository : BaseRepository<News>, INewsRepository
    {
        public NewsRepository(IOptions<DbSettings> connectionString)
            : base(connectionString.Value.ConnectionString)
        {

        }

        public async Task<IEnumerable<News>> FindAsync(FilterDefinition<News> filter, int count, int skip = 0)
        {
            var news = await Collection.Find(filter)
                             .Skip(skip)
                             .Limit(count)
                             .SortByDescending(n => n.CreatedDate)
                             .ToListAsync();

            return news;
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            FieldDefinition<News, Category> field = nameof(Category);
            var filter = Builders<News>.Filter.Empty;

            var cursor = await Collection.DistinctAsync(field, filter);
            await cursor.MoveNextAsync();

            return cursor.Current;
        }

        public async Task BulkInsertAsync(IEnumerable<News> news)
        {
            await Collection.InsertManyAsync(news);
        } 
    }
}
