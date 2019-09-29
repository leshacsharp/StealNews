using StealNews.Model.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StealNews.DataProvider.Repositories.Abstraction
{
    public interface INewsRepository : IBaseRepository<News>
    {
        Task BulkInsertAsync(IEnumerable<News> news);
    }
}
