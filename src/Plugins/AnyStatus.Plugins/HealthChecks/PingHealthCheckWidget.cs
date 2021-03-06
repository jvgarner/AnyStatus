﻿using AnyStatus.API.Widgets;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AnyStatus.Plugins.HealthChecks
{
    [DisplayName("Ping")]
    [Category("Health Checks")]
    [Description("Monitor the reachability of a host address by sending periodic health checks")]
    public class PingHealthCheckWidget : StatusWidget, IPollable, IStandardWidget
    {
        [Required]
        [Category("Ping")]
        [Description("The host name or IP address.")]
        public string Host { get; set; }
    }
}
