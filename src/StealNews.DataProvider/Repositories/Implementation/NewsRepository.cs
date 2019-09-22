using Microsoft.Extensions.Options;
using StealNews.DataProvider.Repositories.Abstraction;
using StealNews.DataProvider.Settings;
using StealNews.Model.Entities;

namespace StealNews.DataProvider.Repositories.Implementation
{
    public class NewsRepository : BaseRepository<News>, INewsRepository
    {
        public NewsRepository(IOptions<DbSettings> connectionString)
            : base(connectionString.Value.ConnectionString)
        {

        }
    }
}
