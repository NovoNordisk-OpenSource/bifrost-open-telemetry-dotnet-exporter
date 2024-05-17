namespace NovoNordisk.OpenTelemetry.Exporter.Bifrost.Authorization;

internal enum AuthorizationOptions
{
    NoAuth,
    ServicePrincipal,
    SystemAssignedIdentity,
    UserAssignedIdentity
}
