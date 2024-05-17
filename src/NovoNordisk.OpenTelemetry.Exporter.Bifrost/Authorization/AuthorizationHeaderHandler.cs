using System.Net.Http.Headers;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.AppConfig;
using Microsoft.Identity.Web;

namespace NovoNordisk.OpenTelemetry.Exporter.Bifrost.Authorization;

/// <summary>
/// Represents a message handler that adds an authorization header to outgoing HTTP requests for telemetry purposes.
/// </summary>
internal class AuthorizationHeaderHandler(HttpMessageHandler innerHandler, MicrosoftIdentityOptions identityOptions, OpenTelemetryExporterOptions otlpExporterOptions, AuthorizationOptions options = AuthorizationOptions.ServicePrincipal) : DelegatingHandler(innerHandler)
{
    private static readonly TimeSpan MinimumValidityPeriod = TimeSpan.FromMinutes(2);

    private AuthenticationResult? _bearerTelemetryAuthenticationResult = null;

    private readonly MicrosoftIdentityOptions _identityOptions = identityOptions;

    private readonly OpenTelemetryExporterOptions _otlpExporterOptions = otlpExporterOptions;

    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {       
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        request.Headers.Authorization = new AuthenticationHeaderValue(
            Constants.Bearer,
            GetAccessToken(Constants.Bearer)
        );

        return base.Send(request, cancellationToken);
    }

    private string? GetAccessToken(string tokenType)
    {
        var scope = tokenType switch
        {
            Constants.Bearer => _otlpExporterOptions.BifrostEnvironmentId ?? throw new ArgumentNullException($"{_otlpExporterOptions.BifrostEnvironmentId} is null."),
            _ => throw new ArgumentOutOfRangeException(nameof(tokenType), tokenType, null),
        };

        var authenticationResult = tokenType switch
        {
            Constants.Bearer => _bearerTelemetryAuthenticationResult,
            _ => throw new ArgumentOutOfRangeException(nameof(tokenType), tokenType, null)
        };

        bool tokenExpiredOrAboutToExpire;

        if (authenticationResult != null)
            tokenExpiredOrAboutToExpire = authenticationResult?.ExpiresOn < DateTimeOffset.UtcNow + MinimumValidityPeriod;
        else
            tokenExpiredOrAboutToExpire = true;

        if (tokenExpiredOrAboutToExpire)
        {
            authenticationResult = GetAuthenticationResultAsync(_identityOptions, options, scope).Result;
        }

        if (authenticationResult == null)
        {
            return "TokenAcquisitionFailed";
        }

        _bearerTelemetryAuthenticationResult = tokenType switch
        {
            Constants.Bearer => authenticationResult,
            _ => throw new ArgumentOutOfRangeException(nameof(tokenType), tokenType, null),
        };

        return authenticationResult?.AccessToken;
    }

    private static async Task<AuthenticationResult?> GetAuthenticationResultAsync(MicrosoftIdentityOptions identityOptions, AuthorizationOptions options, string scope)
    {
        IConfidentialClientApplication confidentialClientApplication;
        IManagedIdentityApplication managedIdApplication;
        AuthenticationResult? authenticationResult;

        switch (options)
        {
            case AuthorizationOptions.ServicePrincipal:
                if (identityOptions.ClientId == null || identityOptions.ClientSecret == null || identityOptions.TenantId == null)
                    throw new ArgumentNullException(
                        $"Identity options {identityOptions.ClientId}, {identityOptions.ClientSecret}, or {identityOptions.TenantId} is null."
                    );

                confidentialClientApplication = ConfidentialClientApplicationBuilder
                    .Create(identityOptions.ClientId)
                    .WithClientSecret(identityOptions.ClientSecret)
                    .WithAuthority($"https://login.microsoftonline.com/{identityOptions.TenantId}")
                    .WithExperimentalFeatures()
                    .Build();

                authenticationResult = await confidentialClientApplication
                    .AcquireTokenForClient([$"{scope}/.default"])
                    .ExecuteAsync()
                    .ConfigureAwait(false);

                break;

            case AuthorizationOptions.SystemAssignedIdentity:
                managedIdApplication = ManagedIdentityApplicationBuilder
                    .Create(ManagedIdentityId.SystemAssigned)
                    // Azure Container Apps does not work without this
                    .WithExperimentalFeatures()
                    .Build();

                authenticationResult = await managedIdApplication
                    .AcquireTokenForManagedIdentity(scope)
                    .ExecuteAsync()
                    .ConfigureAwait(false);

                break;

            case AuthorizationOptions.UserAssignedIdentity:
                if (identityOptions.UserAssignedManagedIdentityClientId == null)
                    throw new ArgumentNullException(
                        $"Identity option {identityOptions.UserAssignedManagedIdentityClientId} is null."
                    );

                managedIdApplication = ManagedIdentityApplicationBuilder
                    .Create(ManagedIdentityId.WithUserAssignedResourceId(identityOptions.UserAssignedManagedIdentityClientId))
                    // Azure Container Apps does not work without this
                    .WithExperimentalFeatures()
                    .Build();

                authenticationResult = await managedIdApplication
                    .AcquireTokenForManagedIdentity(scope)
                    .ExecuteAsync()
                    .ConfigureAwait(false);

                break;

            case AuthorizationOptions.NoAuth:
                authenticationResult = null;
                
                break;

            default:
                throw new ArgumentException($"Invalid AuthorizationEnvironmentOptions value {options}");
        }

        return authenticationResult;
    }
}
