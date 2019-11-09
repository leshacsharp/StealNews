using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StealNews.Core.Ioc;
using StealNews.Core.Settings;
using StealNews.DataProvider.Ioc;
using StealNews.DataProvider.Settings;
using StealNews.Common.Logging;
using StealNews.WebAPI.Filters;
using StealNews.WebAPI.Middlewares;

namespace StealNews.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options => options.Filters.Add(typeof(ExceptionFilter)))
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                    .AddMvcOptions(options => options.EnableEndpointRouting = false);
                           
            services.AddDbDependencies();
            services.AddCoreDependencies();

            services.Configure<DbSettings>(Configuration.GetSection(nameof(DbSettings)));
            services.Configure<SourceConfiguration>(Configuration.GetSection(nameof(SourceConfiguration)));
            services.Configure<BackgroundWorkerConfiguration>(Configuration.GetSection(nameof(BackgroundWorkerConfiguration)));
            services.Configure<InfoGeneratorsConfiguration>(Configuration.GetSection(nameof(InfoGeneratorsConfiguration)));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            Logger.Configure(loggerFactory);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseHsts();
            }

            app.UseWebSockets();
            app.UseNewsWebSockets("/newsEndpoint");

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
