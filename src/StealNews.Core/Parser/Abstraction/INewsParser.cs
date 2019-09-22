using StealNews.Model.Entities;
using System.Threading.Tasks;

namespace StealNews.Core.Parser.Abstraction
{
    public interface INewsParser
    {
        Task<News> ParseAsync(string source);
    }
}
