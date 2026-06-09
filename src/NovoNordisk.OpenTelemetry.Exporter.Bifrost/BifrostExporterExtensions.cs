namespace NovoNordisk.OpenTelemetry.Exporter.Bifrost;

using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using NovoNordisk.OpenTelemetry.Exporter.Bifrost.Authorization;
using global::OpenTelemetry.Exporter;
using global::OpenTelemetry.Logs;
using global::OpenTelemetry.Metrics;
using global::OpenTelemetry.Trace;

/// <summary>
/// Extensions for instrumenting Bifrost exporters in Open Telemetry.
/// </summary>
public static class BifrostExporterExtensions
{
    /// <summary>
    /// Adds a Bifrost exporter the OpenTelemetry log exporter <see cref="ILoggerProvider"/>.
    /// </summary>
    /// <param name="loggerOptions"><see cref="OpenTelemetryLoggerOptions"/> options to use.</param>
    /// <param name="bifrostOptions">Required options for exporting to Bifrost <see cref="BifrostExporterOptions"/>.</param>
    /// <returns>The instance of <see cref="OpenTelemetryLoggerOptions"/> to chain the calls.</returns>
    public static OpenTelemetryLoggerOptions AddBifrostExporter(
        this OpenTelemetryLoggerOptions loggerOptions,
        BifrostOptions bifrostOptions)
        => AddBifrostExporter(
            loggerOptions,
            bifrostOptions.Endpoint,
            bifrostOptions.BifrostEnvironmentId,
            bifrostOptions.IdentityOptions,
            bifrostOptions.AuthorizationMode);

    /// <summary>
    /// Adds a Bifrost exporter the OpenTelemetry log exporter <see cref="ILoggerProvider"/>.
    /// </summary>
    /// <param name="loggerOptions"><see cref="OpenTelemetryLoggerOptions"/> options to use.</param>
    /// <param name="bifrostEndpoint">OTLP Bifrost endpoint. '/logs' will be appended if it is not given.</param>
    /// <param name="bifrostEnvironmentId">Bifrost environment ID</param>
    /// <param name="identityOptions">MicrosoftIdentityOptions whose fields are read according to <paramref name="authorizationMode"/>.</param>
    /// <param name="authorizationMode">The credential flow to use. Defaults to <see cref="BifrostAuthorizationMode.ServicePrincipal"/> for backward compatibility.</param>
    /// <returns>The instance of <see cref="OpenTelemetryLoggerOptions"/> to chain the calls.</returns>
    public static OpenTelemetryLoggerOptions AddBifrostExporter(
        this OpenTelemetryLoggerOptions loggerOptions,
        string bifrostEndpoint,
        string bifrostEnvironmentId,
        MicrosoftIdentityOptions identityOptions,
        BifrostAuthorizationMode authorizationMode = BifrostAuthorizationMode.ServicePrincipal)
    {
        var logsEndpoint = bifrostEndpoint.EndsWith("/logs") ? bifrostEndpoint : $"{bifrostEndpoint}/logs";

        return loggerOptions.AddOtlpExporter(BifrostExporterOptions(
            logsEndpoint,
            bifrostEnvironmentId,
            identityOptions,
            authorizationMode));
    }

    /// <summary>
    /// Adds Bifrost OpenTelemetry Protocol (OTLP) exporter to the <see cref="TracerProviderBuilder"/>.
    /// </summary>
    /// <param name="tracerProviderBuilder"><see cref="TracerProviderBuilder"/> builder to use.</param>
    /// <param name="bifrostOptions">Required options for exporting to Bifrost <see cref="BifrostExporterOptions"/>.</param>
    /// <returns>The instance of <see cref="TracerProviderBuilder"/> to chain the calls.</returns>
    public static TracerProviderBuilder AddBifrostExporter(
        this TracerProviderBuilder tracerProviderBuilder,
        BifrostOptions bifrostOptions)
        => AddBifrostExporter(
            tracerProviderBuilder,
            bifrostOptions.Endpoint,
            bifrostOptions.BifrostEnvironmentId,
            bifrostOptions.IdentityOptions,
            bifrostOptions.AuthorizationMode);

    /// <summary>
    /// Adds Bifrost OpenTelemetry Protocol (OTLP) exporter to the <see cref="TracerProviderBuilder"/>.
    /// </summary>
    /// <param name="tracerProviderBuilder"><see cref="TracerProviderBuilder"/> builder to use.</param>
    /// <param name="bifrostEndpoint">OTLP Bifrost endpoint. '/traces' will be appended if it is not given.</param>
    /// <param name="bifrostEnvironmentId">Bifrost environment ID</param>
    /// <param name="identityOptions">MicrosoftIdentityOptions whose fields are read according to <paramref name="authorizationMode"/>.</param>
    /// <param name="authorizationMode">The credential flow to use. Defaults to <see cref="BifrostAuthorizationMode.ServicePrincipal"/> for backward compatibility.</param>
    /// <returns>The instance of <see cref="TracerProviderBuilder"/> to chain the calls.</returns>
    public static TracerProviderBuilder AddBifrostExporter(
        this TracerProviderBuilder tracerProviderBuilder,
        string bifrostEndpoint,
        string bifrostEnvironmentId,
        MicrosoftIdentityOptions identityOptions,
        BifrostAuthorizationMode authorizationMode = BifrostAuthorizationMode.ServicePrincipal)
    {
        var tracesEndpoint = bifrostEndpoint.EndsWith("/traces") ? bifrostEndpoint : $"{bifrostEndpoint}/traces";

        return tracerProviderBuilder.AddOtlpExporter(BifrostExporterOptions(
            tracesEndpoint,
            bifrostEnvironmentId,
            identityOptions,
            authorizationMode));
    }

    /// <summary>
    /// Adds Bifrost OpenTelemetry Protocol (OTLP) exporter to the <see cref="MeterProviderBuilder"/>.
    /// </summary>
    /// <param name="meterProviderBuilder"><see cref="MeterProviderBuilder"/> builder to use.</param>
    /// <param name="bifrostOptions">Required options for exporting to Bifrost <see cref="BifrostExporterOptions"/>.</param>
    /// <returns>The instance of <see cref="MeterProviderBuilder"/> to chain the calls.</returns>
    public static MeterProviderBuilder AddBifrostExporter(
        this MeterProviderBuilder meterProviderBuilder,
        BifrostOptions bifrostOptions)
        => AddBifrostExporter(
            meterProviderBuilder,
            bifrostOptions.Endpoint,
            bifrostOptions.BifrostEnvironmentId,
            bifrostOptions.IdentityOptions,
            bifrostOptions.AuthorizationMode);

    /// <summary>
    /// Adds Bifrost OpenTelemetry Protocol (OTLP) exporter to the <see cref="MeterProviderBuilder"/>.
    /// </summary>
    /// <param name="meterProviderBuilder"><see cref="MeterProviderBuilder"/> builder to use.</param>
    /// <param name="bifrostEndpoint">OTLP Bifrost endpoint. '/metrics' will be appended if it is not given.</param>
    /// <param name="bifrostEnvironmentId">Bifrost environment ID</param>
    /// <param name="identityOptions">MicrosoftIdentityOptions whose fields are read according to <paramref name="authorizationMode"/>.</param>
    /// <param name="authorizationMode">The credential flow to use. Defaults to <see cref="BifrostAuthorizationMode.ServicePrincipal"/> for backward compatibility.</param>
    /// <returns>The instance of <see cref="MeterProviderBuilder"/> to chain the calls.</returns>
    public static MeterProviderBuilder AddBifrostExporter(this MeterProviderBuilder meterProviderBuilder,
        string bifrostEndpoint,
        string bifrostEnvironmentId,
        MicrosoftIdentityOptions identityOptions,
        BifrostAuthorizationMode authorizationMode = BifrostAuthorizationMode.ServicePrincipal)
    {
        var metricsEndpoint = bifrostEndpoint.EndsWith("/metrics") ? bifrostEndpoint : $"{bifrostEndpoint}/metrics";

        return meterProviderBuilder.AddOtlpExporter(BifrostExporterOptions(
            metricsEndpoint,
            bifrostEnvironmentId,
            identityOptions,
            authorizationMode));
    }

    private static Action<OtlpExporterOptions> BifrostExporterOptions(
        string endpoint,
        string bifrostEnvironmentId,
        MicrosoftIdentityOptions identityOptions,
        BifrostAuthorizationMode authorizationMode)
    {
        return exporterOptions =>
        {
            exporterOptions.Endpoint = new Uri(endpoint);
            exporterOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
            exporterOptions.HttpClientFactory = ExporterClientFactory(endpoint, bifrostEnvironmentId, identityOptions, authorizationMode);
        };
    }

    private static Func<HttpClient> ExporterClientFactory(
        string endpoint,
        string bifrostEnvironmentId,
        MicrosoftIdentityOptions identityOptions,
        BifrostAuthorizationMode authorizationMode)
    {
        return () =>
        {
            var innerHandler = new HttpClientHandler();
            var client = new HttpClient(
                new AuthorizationHeaderHandler(
                    innerHandler,
                    identityOptions,
                    new OpenTelemetryExporterOptions
                    {
                        Endpoint = endpoint,
                        BifrostEnvironmentId = bifrostEnvironmentId
                    },
                    authorizationMode
                )
            )
            {
                Timeout = TimeSpan.FromMilliseconds(10000)  //TODO: Make this configurable
            };

            return client;
        };
    }
}
