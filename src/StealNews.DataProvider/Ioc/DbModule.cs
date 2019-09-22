using Microsoft.Extensions.DependencyInjection;
using StealNews.DataProvider.Repositories.Abstraction;
using StealNews.DataProvider.Repositories.Implementation;

namespace StealNews.DataProvider.Ioc
{
    public static class DbModule
    {
        public static void AddDbDependencies(this IServiceCollection services)
        {
            services.AddScoped<INewsRepository, NewsRepository>();
        }
    }
}
