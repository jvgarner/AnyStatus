﻿using AnyStatus.API.Services;
using AnyStatus.API.Widgets;
using AnyStatus.Plugins.Azure.API;
using AnyStatus.Plugins.Azure.API.Endpoints;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AnyStatus.Plugins.Azure.DevOps.PullRequests
{
    public class AzureDevOpsPullRequestsQuery : AsyncMetricQuery<AzureDevOpsPullRequestsWidget>, IEndpointHandler<IAzureDevOpsEndpoint>
    {
        private readonly IMapper _mapper;
        private readonly IDispatcher _dispatcher;

        public AzureDevOpsPullRequestsQuery(IDispatcher dispatcher, IMapper mapper)
        {
            _mapper = mapper;
            _dispatcher = dispatcher;
        }

        public IAzureDevOpsEndpoint Endpoint { get; set; }

        protected async override Task Handle(MetricRequest<AzureDevOpsPullRequestsWidget> request, CancellationToken cancellationToken)
        {
            var api = new AzureDevOpsApi(Endpoint);

            var pullRequests = await api.GetPullRequestsAsync(request.Context.Account, request.Context.Project).ConfigureAwait(false);

            request.Context.Value = pullRequests.Count;

            if (pullRequests is null || pullRequests.Count == 0)
            {
                request.Context.Status = Status.None;

                _dispatcher.InvokeAsync(request.Context.Clear);
            }
            else
            {
                _dispatcher.InvokeAsync(() 
                    => new AzureDevOpsPullRequestSynchronizer(_mapper, request.Context)
                            .Synchronize(pullRequests.Value.ToList(), request.Context.OfType<AzureDevOpsPullRequestWidget>().ToList()));
            }

            request.Context.Status = Status.OK;
        }
    }
}
