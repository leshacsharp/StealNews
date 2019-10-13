using StealNews.Model.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StealNews.DataProvider.Repositories.Abstraction
{
    public interface INewsRepository : IBaseRepository<News>
    {
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Task<IEnumerable<Source>> GetSourcesAsync();
        Task BulkInsertAsync(IEnumerable<News> news);
    }
}
