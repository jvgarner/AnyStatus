using AnyStatus.API.Attributes;
using AnyStatus.API.Widgets;
using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AnyStatus.Plugins.Octopus
{
    [Category("Octopus")]
    [DisplayName("Octopus Server Health")]
    [Description("Octopus Server Health Monitor")]
    public class OctopusServerHealthWidget : StatusWidget, IRequireEndpoint<OctopusEndpoint>, IStandardWidget, IPollable, IProgress
    {
        [Required]
        [EndpointSource]
        [DisplayName("Endpoint")]
        public string EndpointId { get; set; }
    }
}
