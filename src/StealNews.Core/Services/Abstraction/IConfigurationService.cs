using StealNews.Model.Models.Service.Configuration;

namespace StealNews.Core.Services.Abstraction
{
    public interface IConfigurationService
    {
        AppConfiguration Get();
    }
}
