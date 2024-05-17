using Microsoft.Identity.Web;

namespace NovoNordisk.OpenTelemetry.Exporter.Bifrost;

/// <summary>
/// Options required for Bifrost exporters.
/// </summary>
public class BifrostOptions
{
    /// <summary>
    /// Bifrost environment identifier.
    /// </summary>
    public required string BifrostEnvironmentId { get; set; }
    
    /// <summary>
    /// Bifrost OTLP endpoint.
    /// </summary>
    public required string Endpoint { get; set; }
    
    /// <summary>
    /// AD options.
    /// </summary>
    public required MicrosoftIdentityOptions IdentityOptions { get; set; }
}
