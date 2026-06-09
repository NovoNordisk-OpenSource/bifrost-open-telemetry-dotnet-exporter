namespace NovoNordisk.OpenTelemetry.Exporter.Bifrost;

using Microsoft.Identity.Web;

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
    /// AD options. Which fields are read depends on <see cref="AuthorizationMode"/>:
    /// <list type="bullet">
    /// <item><description><see cref="BifrostAuthorizationMode.ServicePrincipal"/> reads <c>ClientId</c>, <c>ClientSecret</c> and <c>TenantId</c>.</description></item>
    /// <item><description><see cref="BifrostAuthorizationMode.UserAssignedIdentity"/> reads <c>UserAssignedManagedIdentityClientId</c>.</description></item>
    /// <item><description><see cref="BifrostAuthorizationMode.SystemAssignedIdentity"/> and <see cref="BifrostAuthorizationMode.NoAuth"/> read no fields.</description></item>
    /// </list>
    /// </summary>
    public required MicrosoftIdentityOptions IdentityOptions { get; set; }

    /// <summary>
    /// Selects the credential flow used to acquire bearer tokens for Bifrost. Defaults to
    /// <see cref="BifrostAuthorizationMode.ServicePrincipal"/> for backward compatibility.
    /// </summary>
    public BifrostAuthorizationMode AuthorizationMode { get; set; } = BifrostAuthorizationMode.ServicePrincipal;
}
