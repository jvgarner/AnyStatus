using AnyStatus.API.Endpoints;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AnyStatus.Plugins.Octopus
{
    [DisplayName("Octopus")]
    public class OctopusEndpoint : Endpoint
    {
        public OctopusEndpoint()
        {
            Name = "Octopus";
        }

        [Description("Required. API Key")]
        [Required]
        public string ApiKey { get; set; }

        [DisplayName("Required. Machine Name")]
        [Required]
        public string MachineName { get; set; }
    }
}
