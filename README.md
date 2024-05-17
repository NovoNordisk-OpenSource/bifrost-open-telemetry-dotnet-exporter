# Digital Checklist Bifrost Exporter
The content of this library is a set of OpenTelemetry instrumentation extensions for exporting telemetry to Bifrost.
This library is intended to be used in conjunction with the OpenTelemetry SDK and exporter libraries.

It is based on https://github.com/NovoNordisk-OpenSource/heimdall-templates-dotnet-microservice/blob/main/src/Heimdall.Templates.Dotnet.Microservice.Infrastructure/OpenTelemetry

# Example
## Log Exporter
Here is an example with a dummy configuration that will export open telemetry logs to Bifrost using a service principal.

```csharp
builder.Logging.AddOpenTelemetry(options =>
{
    options.AddBifrostExporter(
       bifrostEndpoint: "https://your.bifrost.endpoint/otlp/http/v1",
       bifrostEnvironmentId: "d9a8719a-8bc2-4829-a078-231df13fd125",
       identityOptions: new MicrosoftIdentityOptions
           {
               Instance = "https://login.microsoftonline.com/",
               ClientId = "8be5cb8b-3a8d-47bf-9b70-660963b311ef",
               ClientSecret = "ThisIsYourClientSecret",
               TenantId = "c39886aa-7273-4937-9cad-53b86940713f")
           });
});
```

The `BifrostOptions` can also be used in case you want to use the same configuration for both logs, traces and metrics.
```csharp
var bifrostOptions = new BifrostOptions
{
    bifrostEndpoint: "https://your.bifrost.endpoint/otlp/http/v1",
    bifrostEnvironmentId: "d9a8719a-8bc2-4829-a078-231df13fd125",
    identityOptions: new MicrosoftIdentityOptions
        {
            Instance = "https://login.microsoftonline.com/",
            ClientId = "8be5cb8b-3a8d-47bf-9b70-660963b311ef",
            ClientSecret = "ThisIsYourClientSecret",
            TenantId = "c39886aa-7273-4937-9cad-53b86940713f")
        });    
};

builder.Logging.AddOpenTelemetry(options =>
{
    options.AddBifrostExporter(bifrostOptions);
});

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing.AddBifrostExporter(bifrostOptions);
    }
    .WithMetrics(metrics =>
    {
        metrics.AddBifrostExporter(bifrostOptions);
    };
```

# TODO
- We could probably use BifrostOptions in the private methods of BifrostExporter and make the arguments simpler.
- Add tests
