using AnyStatus.API.Widgets;
using MediatR;
using Microsoft.Extensions.Logging;
using Octopus.Client;
using Octopus.Client.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.Octopus
{
    public class OctopusServerHealthCheck : AsyncStatusCheck<OctopusServerHealthWidget>, IEndpointHandler<OctopusEndpoint>
    {
        private readonly ILogger _logger;

        public OctopusServerHealthCheck(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public OctopusEndpoint Endpoint { get; set; }

        protected override Task Handle(StatusRequest<OctopusServerHealthWidget> request, CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Testing {request.Context.Name}");
            Status status;
            try
            {
                var endpoint = new OctopusServerEndpoint(Endpoint.Address, Endpoint.ApiKey);
                var repo = new OctopusRepository(endpoint);
                var machine = repo.Machines.FindByName(Endpoint.MachineName);

                if (machine != null)
                {
                    if (machine.HealthStatus != MachineModelHealthStatus.Healthy || machine.Status != MachineModelStatus.Online)
                    {
                        status = Status.Failed;
                        status.Metadata.DisplayName = $"Machine Offline or Unhealthy: {Endpoint.Address} - {Endpoint.MachineName}";
                    }
                    else
                    {
                        status = machine.IsInProcess ? Status.Running : Status.OK;
                    }
                }
                else
                {
                    status = Status.Invalid;
                    status.Metadata.DisplayName = $"Machine Not Found: {Endpoint.Address} - {Endpoint.MachineName}";
                }
            }
            catch (Exception ex)
            {
                status = Status.Error;
                status.Metadata.DisplayName = ex.ToString();
            }

            request.Context.Status = status;
            return Unit.Task;
        }
    }
}
