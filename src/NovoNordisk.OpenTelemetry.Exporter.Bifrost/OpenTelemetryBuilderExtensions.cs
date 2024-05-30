using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace NovoNordisk.OpenTelemetry.Exporter.Bifrost;

/// <summary>
/// Extensions for instrumenting Bifrost in Open Telemetry.
/// </summary>
public static class OpenTelemetryBuilderExtensions
{
    /// <summary>
    /// This extension adds Bifrost exporters to the OpenTelemetryBuilder. Logs, Traces and Metrics are instrumented.
    /// </summary>
    /// <param name="builder"><see cref="IOpenTelemetryBuilder"/> builder to use.</param>
    /// <param name="bifrostOptions">Required options for exporting to Bifrost <see cref="BifrostOptions"/>.</param>
    /// <returns>The instance of <see cref="IOpenTelemetryBuilder"/> to chain the calls.</returns>
    public static IOpenTelemetryBuilder UseBifrost(this IOpenTelemetryBuilder builder, BifrostOptions bifrostOptions)
    {
        return builder.UseBifrost(bifrostOptions, Array.Empty<string>(), Array.Empty<string>());
    }
    
    /// <summary>
    /// This extension adds Bifrost exporters to the OpenTelemetryBuilder. Logs, Traces and Metrics are instrumented.
    /// </summary>
    /// <param name="builder"><see cref="IOpenTelemetryBuilder"/> builder to use.</param>
    /// <param name="bifrostOptions">Required options for exporting to Bifrost <see cref="BifrostOptions"/>.</param>
    /// <param name="activitySourceNames">List of custom activity names. Used to add custom spans to traces.</param>
    /// <param name="meterNames">List of custom meters.</param>
    /// <returns>The instance of <see cref="IOpenTelemetryBuilder"/> to chain the calls.</returns>
    public static IOpenTelemetryBuilder UseBifrost(this IOpenTelemetryBuilder builder, BifrostOptions bifrostOptions,
        string[] activitySourceNames, string[] meterNames)
    {
        builder.WithTracing(tracing =>
        {
            tracing
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddSource("Azure.*")
                .AddSource(activitySourceNames);

            tracing.AddBifrostExporter(bifrostOptions);
        });
        
        builder.WithMetrics(metrics =>
        {
            metrics
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddRuntimeInstrumentation()
                .AddMeter(meterNames);
            
            metrics.AddBifrostExporter(bifrostOptions);
        });
        
        builder.Services.AddLogging(logging =>
        {
            logging.AddOpenTelemetry(options =>
            {
                options.IncludeScopes = true;
                options.IncludeFormattedMessage = true;
                options.AddBifrostExporter(bifrostOptions);
            });
        });
        
        return builder;
    }
}