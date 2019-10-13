using StealNews.Model.Dto;
using StealNews.Model.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StealNews.DataProvider.Repositories.Abstraction
{
    public interface INewsRepository : IBaseRepository<News>
    {
        IEnumerable<CategoryDto> GetCategories();
        IEnumerable<SourceDto> GetSources();
        Task BulkInsertAsync(IEnumerable<News> news);
    }
}
