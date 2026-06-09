namespace NovoNordisk.OpenTelemetry.Exporter.Bifrost;

/// <summary>
/// Selects the credential flow used by the Bifrost exporter to acquire bearer tokens.
/// </summary>
public enum BifrostAuthorizationMode
{
    /// <summary>
    /// No authorization header is attached. Intended for scenarios where the exporter is
    /// behind another component that handles authentication, or for local development against
    /// an unauthenticated collector.
    /// </summary>
    NoAuth,

    /// <summary>
    /// Authenticate as an Entra ID confidential client using <c>ClientId</c>,
    /// <c>ClientSecret</c>, and <see cref="Microsoft.Identity.Web.MicrosoftIdentityOptions.TenantId"/>
    /// from the supplied <see cref="Microsoft.Identity.Web.MicrosoftIdentityOptions"/>. This is
    /// the historical default and is preserved for backward compatibility.
    /// </summary>
    ServicePrincipal,

    /// <summary>
    /// Authenticate using the host's system-assigned managed identity. No credential fields are
    /// read from <see cref="Microsoft.Identity.Web.MicrosoftIdentityOptions"/>.
    /// </summary>
    SystemAssignedIdentity,

    /// <summary>
    /// Authenticate using a user-assigned managed identity. The identity's client id is read
    /// from <c>UserAssignedManagedIdentityClientId</c> on the supplied
    /// <see cref="Microsoft.Identity.Web.MicrosoftIdentityOptions"/>.
    /// </summary>
    UserAssignedIdentity,

    /// <summary>
    /// Authenticate as an Entra ID confidential client whose credential is a federated assertion
    /// produced by a user-assigned managed identity attached to the host (e.g. an Azure Container App).
    /// The bearer token issued is for the app registration (not the UAMI) — the UAMI's only role is
    /// to sign the assertion. Use this when the receiving system (e.g. Bifrost / Grafana) authorizes
    /// based on the registered app and you want to retire client-secret storage.
    /// </summary>
    /// <remarks>
    /// Requires (1) <c>ClientId</c> and <c>TenantId</c> on the supplied
    /// <see cref="Microsoft.Identity.Web.MicrosoftIdentityOptions"/> identifying the app registration,
    /// and (2) <c>UserAssignedManagedIdentityClientId</c> identifying the UAMI that signs the
    /// assertion. The app registration must have a federated credential trusting that UAMI with
    /// audience <c>api://AzureADTokenExchange</c>.
    /// </remarks>
    FederatedManagedIdentity
}
