using StealNews.Model.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StealNews.Core.InfoGenerator.Abstraction
{
    public interface IInfoGenerator
    {
        Task ProcessAsync(IEnumerable<News> news);
    }
}
