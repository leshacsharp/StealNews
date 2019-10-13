using StealNews.Core.Services.Abstraction;
using StealNews.DataProvider.Repositories.Abstraction;
using StealNews.Model.Models.Service.Configuration;

namespace StealNews.Core.Services.Implementation
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly INewsRepository _newsRepository;
        public ConfigurationService(INewsRepository newsRepository)
        {
            _newsRepository = newsRepository;
        }

        public AppConfiguration Get()
        {
            var categories = _newsRepository.GetCategories();
            var sources = _newsRepository.GetSources();

            var appConfiguration = new AppConfiguration()
            {
                Categories = categories,
                Sources = sources
            };

            return appConfiguration;
        }
    }
}
