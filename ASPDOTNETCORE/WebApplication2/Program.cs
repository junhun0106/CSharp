using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace WebApplication2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var isService = !args.Contains("--console", System.StringComparer.Ordinal);
            var build = CreateHostBuilder(args).Build();
            if (isService) {
                build.RunAsService();
            } else {
                build.Run();
            }
        }

        public static IWebHostBuilder CreateHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
#if DEBUG
                .ConfigureServices((_, services) => services.AddHostedService<InputHostedService>())
#endif
                .UseStartup<Startup>();
    }
}
