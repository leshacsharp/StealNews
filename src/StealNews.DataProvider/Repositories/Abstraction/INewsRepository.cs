using MongoDB.Driver;
using StealNews.Model.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StealNews.DataProvider.Repositories.Abstraction
{
    public interface INewsRepository : IBaseRepository<News>
    {
        Task<IEnumerable<News>> FindAsync(FilterDefinition<News> filter, int count = 100, int skip = 0);
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Task BulkInsertAsync(IEnumerable<News> news);
    }
}
