using StealNews.Core.Models;
using StealNews.Model.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StealNews.Core.Services.Abstraction
{
    public interface INewsService
    {
        Task<IEnumerable<News>> GenerateNewsAsync();
        IEnumerable<News> Find(NewsFindFilter filter);
    }
}
