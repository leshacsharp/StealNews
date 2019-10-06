using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StealNews.Core.Ioc;
using StealNews.Core.Services.Implementation;
using StealNews.Core.Settings;
using StealNews.DataProvider.Ioc;
using StealNews.DataProvider.Settings;

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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                             .AddMvcOptions(options => options.EnableEndpointRouting = false);
                           
            services.AddDbDependencies();
            services.AddCoreDependencies();

            services.Configure<DbSettings>(Configuration.GetSection(nameof(DbSettings)));
            services.Configure<SourceConfiguration>(Configuration.GetSection(nameof(SourceConfiguration)));
            services.Configure<BackgroundWorkersConfiguration>(Configuration.GetSection(nameof(BackgroundWorkersConfiguration)));

            services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, BackgroundNewsGenerator>(p => new BackgroundNewsGenerator(services.BuildServiceProvider()));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
