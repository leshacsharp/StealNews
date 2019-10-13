using StealNews.Model.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using StealNews.Model.Models.Service.News;

namespace StealNews.Core.Services.Abstraction
{
    public interface INewsService
    {
        Task<IEnumerable<News>> GenerateNewsAsync();
        IEnumerable<News> Find(NewsFindFilter filter);
    }
}
