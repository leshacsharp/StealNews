using System.Collections.Generic;
using System.Threading.Tasks;

namespace StealNews.Core.SourceValidators.Abstraction
{
    public interface ISourceValidator
    {
        Task<IEnumerable<string>> ValidateAsync(IEnumerable<string> sources);
    }
}
