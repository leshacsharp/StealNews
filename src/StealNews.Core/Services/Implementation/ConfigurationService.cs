using StealNews.Core.Services.Abstraction;
using StealNews.DataProvider.Repositories.Abstraction;
using StealNews.Model.Entities;
using StealNews.Model.Models.Service.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace StealNews.Core.Services.Implementation
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly INewsRepository _newsRepository;
        public ConfigurationService(INewsRepository newsRepository)
        {
            _newsRepository = newsRepository;
        }

        public async Task<AppConfiguration> GetAsync()
        {
            var categories = await _newsRepository.GetCategoriesAsync();
            var sources = await _newsRepository.GetSourcesAsync();

            var distinctCategories = from c in categories
                                     group c by c.Title into gr
                                     select new Category()
                                     {
                                         Title = gr.Key,
                                         SubCategories = gr.SelectMany(p => p.SubCategories)
                                                           .Distinct()
                                     };

            var appConfiguration = new AppConfiguration()
            {
                Categories = distinctCategories,
                Sources = sources
            };

            return appConfiguration;
        }
    }
}
