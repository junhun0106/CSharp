using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ChatService
{
    public class HostedService : IHostedService
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;
        private Server server;

        public HostedService(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<HostedService>();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            server = new Server(_loggerFactory);
            server.Start(15344);

            _logger.LogInformation($"{Program.Version} : instance Start - {DateTime.Now}");

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            server?.Close();
            server = null;
            return Task.CompletedTask;
        }
    }
}
