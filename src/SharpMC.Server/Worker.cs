using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SharpMC.API;

namespace SharpMC.Server
{
    internal sealed class Worker : IHostedService
    {
        private readonly ILogger<Worker> _log;
        private readonly IServer _server;

        public Worker(ILogger<Worker> log, IServer server)
        {
            _log = log;
            _server = server;
        }

        public Task StartAsync(CancellationToken token)
        {
            _log.LogInformation("Starting up...");
            _server.Start();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken token)
        {
            _log.LogInformation("Shutting down...");
            _server.Stop();
            return Task.CompletedTask;
        }
    }
}