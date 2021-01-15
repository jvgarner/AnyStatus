using AnyStatus.API.Attributes;
using AnyStatus.API.Widgets;
using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace AnyStatus.Plugins.Octopus
{
    [Category("Octopus")]
    [DisplayName("Octopus Project Health")]
    [Description("Octopus Project Health Monitor")]
    public class OctopusProjectHealthWidget : StatusWidget, IRequireEndpoint<OctopusEndpoint>, IStandardWidget, IPollable, IProgress
    {
        [Required]
        [EndpointSource]
        [DisplayName("Endpoint")]
        public string EndpointId { get; set; }

        [DisplayName("Project ID")]
        [ReadOnly(true)]
        [XmlIgnore]
        public string ProjectId { get; internal set; }

        [DisplayName("Project Name")]
        [Refresh(nameof(ProjectId))]
        [Required]
        public string ProjectName { get; set; }

        [DisplayName("Environment ID")]
        [XmlIgnore]
        [ReadOnly(true)]
        public string EnvironmentId { get; internal set; }

        [DisplayName("Environment Name")]
        [Required]
        [Refresh(nameof(EnvironmentId))]
        public string EnvironmentName { get; set; }
    }
}
