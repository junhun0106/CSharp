using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace LatencyView
{
    public class Startup
    {
        public readonly IConfiguration Configuration;
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddTransient<StatisticsData>();
            services.AddTransient<IpCheckActionFilter>();

            const string latencyLogSection = "LatencyLog";
            if (Configuration.GetSection(latencyLogSection) != null) {
                var logConfig = new LoggerConfiguration()
                    .ReadFrom.Configuration(Configuration, latencyLogSection);
                LatencyLogger.Init(logConfig);
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseMiddleware<StatisticsMiddleware>();
            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}
