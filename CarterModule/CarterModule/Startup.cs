using Carter;
using Carter.ModelBinding;
using Carter.Response;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace CarterModule
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCarter(configurator: c => {
                c.WithModelBinder<DefaultJsonModelBinder>();
                c.WithResponseNegotiator<NewtonsoftJsonResponseNegotiator>();
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints => {
                endpoints.MapCarter();
            });
        }
    }
}
