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
    public class OctopusProjectHealthCheck : AsyncStatusCheck<OctopusProjectHealthWidget>, IEndpointHandler<OctopusEndpoint>
    {
        private readonly ILogger _logger;

        public OctopusProjectHealthCheck(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public OctopusEndpoint Endpoint { get; set; }

        protected override Task Handle(StatusRequest<OctopusProjectHealthWidget> request, CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Testing {request.Context.Name}");
            Status status;
            try
            {
                var endpoint = new OctopusServerEndpoint(Endpoint.Address, Endpoint.ApiKey);
                var repo = new OctopusRepository(endpoint);
                var machine = repo.Machines.FindByName(Endpoint.MachineName);

                if (string.IsNullOrWhiteSpace(request.Context.ProjectId))
                {
                    var project = repo.Projects.FindByName(request.Context.ProjectName);
                    request.Context.ProjectId = project.Id;
                }
                if (string.IsNullOrWhiteSpace(request.Context.EnvironmentId))
                {
                    var environment = repo.Environments.FindByName(request.Context.EnvironmentName);
                    request.Context.EnvironmentId = environment.Id;
                }

                var deployment = repo.Deployments.FindOne(d => d.ProjectId == request.Context.ProjectId && d.EnvironmentId == request.Context.EnvironmentId);

                //this._webUrl = new Uri(new Uri(this.Url), (deployment?.Link("Web")) ?? "/").AbsoluteUri;

                var task = repo.Tasks.FindOne(t => t.Id == deployment?.TaskId);

                if (task != null)
                {
                    switch (task.State)
                    {
                        case TaskState.Queued:
                            status = Status.Queued;
                            break;
                        case TaskState.Executing:
                            status = Status.Running;
                            break;
                        case TaskState.Failed:
                            status = Status.Failed;
                            break;
                        case TaskState.Canceled:
                            status = Status.Canceled;
                            break;
                        case TaskState.TimedOut:
                            status = Status.Unknown;
                            break;
                        case TaskState.Success:
                            status = Status.OK;
                            break;
                        case TaskState.Cancelling:
                            status = Status.Canceled;
                            break;
                        default:
                            status = Status.Unknown;
                            break;
                    }
                    status.Metadata.DisplayName = task.State.ToString();
                }
                else
                {
                    status = Status.Invalid;
                    status.Metadata.DisplayName = $"Deployment Not Found: {request.Context.ProjectName}-{request.Context.EnvironmentName}";
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
