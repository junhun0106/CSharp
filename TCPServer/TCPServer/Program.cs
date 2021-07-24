using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ChatService
{
    internal static class Program
    {
        public static string Version = "2019. 4.16 16:34";
        public static bool IsWindowService;

        private static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            host.Run();
        }

        static IHostBuilder CreateHostBuilder(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args);
            return builder
                .ConfigureServices((_, services) => services.AddHostedService<HostedService>());
        }
    }
}
