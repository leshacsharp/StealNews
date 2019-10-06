using Microsoft.Extensions.DependencyInjection;
using StealNews.Core.Services.Abstraction;
using StealNews.Core.Services.Implementation;

namespace StealNews.Core.Ioc
{
    public static class CoreModule
    {
        public static void AddCoreDependencies(this IServiceCollection services)
        {
            services.AddScoped<INewsService, NewsService>();
        }
    }
}
