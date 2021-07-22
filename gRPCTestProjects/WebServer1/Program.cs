using gRPCLib.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;

namespace WebServer1
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            System.Console.WriteLine($"run - {Ports.Port1}");

            CreateHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .ConfigureKestrel(option => option.ListenAnyIP(Ports.Port1, listenOptions => listenOptions.Protocols = HttpProtocols.Http2)).UseStartup<Startup>();
    }
}
