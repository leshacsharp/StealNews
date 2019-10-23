using StealNews.Model.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StealNews.Core.Services.Abstraction
{
    public interface INewsGenerator
    {
        Task<IEnumerable<News>> GenerateAsync();
    }
}
