using Microsoft.AspNetCore.Builder;

namespace StealNews.WebAPI.Middlewares
{
    public static class MiddlewareExtensions
    {
        public static void UseNewsWebSockets(this IApplicationBuilder applicationBuilder, string pathMatch)
        {
            applicationBuilder.Map(pathMatch, builder => builder.UseMiddleware<NewsWebSocketMiddleware>());
        }
    }
}
