using Microsoft.Identity.Web;

namespace NovoNordisk.OpenTelemetry.Exporter.Bifrost;

public class BifrostOptions
{
    public required string BifrostEnvironmentId { get; set; }

    public required string Endpoint { get; set; }
    
    public required MicrosoftIdentityOptions IdentityOptions { get; set; }
}
