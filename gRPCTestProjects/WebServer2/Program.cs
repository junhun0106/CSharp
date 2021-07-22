using gRPCLib.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;

namespace WebServer2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            System.Console.WriteLine($"run - {Ports.Port2}");

            CreateHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .ConfigureKestrel(option => option.ListenAnyIP(Ports.Port2, listenOptions => listenOptions.Protocols = HttpProtocols.Http2)).UseStartup<Startup>();
    }
}
