using StealNews.Model.Models.Service.Configuration;
using System.Threading.Tasks;

namespace StealNews.Core.Services.Abstraction
{
    public interface IConfigurationService
    {
        Task<AppConfiguration> GetAsync();
    }
}
