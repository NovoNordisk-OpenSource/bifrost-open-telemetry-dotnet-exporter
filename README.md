# Novo Nordisk C# Bifrost Exporter
The content of this library is a set of OpenTelemetry instrumentation extensions for exporting telemetry to Bifrost.
This library is intended to be used in conjunction with the OpenTelemetry SDK and exporter libraries.

It is based on https://github.com/NovoNordisk-OpenSource/heimdall-templates-dotnet-microservice/blob/main/src/Heimdall.Templates.Dotnet.Microservice.Infrastructure/OpenTelemetry

# Examples
Bifrost can be instrumented in two ways. Either by using the Builder Extensions or by using the Exporter Extensions
## Open Telemetry Builder
Using the Open Telemetry Builder extension is the simplest solution but also the least flexible. It is recommended to use the Exporter Extensions if you need more control over the configuration.

By using the telemetry builder you'll get
- `AspNetCore`, `HttpClient`, `GrpcClient` and `"Azure.*"` traces auto instrumentation.
- `HttpClient`, `AspNetCore`, `Process` and `Runtime` metrics auto instrumentation.
- `IncludeScopes` and `IncludeFormattedMessage` logs. 

You can also add your own activity sources and meters.

```csharp
var bifrostOptions = new BifrostOptions
{
    Endpoint = "https://your.bifrost.endpoint/otlp/http/v1",
    BifrostEnvironmentId = "d9a8719a-8bc2-4829-a078-231df13fd125",
    IdentityOptions = new MicrosoftIdentityOptions
        {
            Instance = "https://login.microsoftonline.com/",
            ClientId = "8be5cb8b-3a8d-47bf-9b70-660963b311ef",
            ClientSecret = "ThisIsYourClientSecret",
            TenantId = "c39886aa-7273-4937-9cad-53b86940713f"
        }    
};
        
builder.Services.AddOpenTelemetry().UseBifrost(bifrostOptions);
```

It is recommended to turn on `EnableActivitySource` in the AppContext. This is needed to get the correct ActivitySource for the OpenTelemetry SDK when using Azure and custom traces. Hopefully this is not needed in future versions of Open Telemetry. 
```csharp
AppContext.SetSwitch("Azure.Experimental.EnableActivitySource", true);
```

It is also recommended to configure a Telemetry Resource. Here's an example for the `OpenTelemetryBuilder`:
```csharp
.ConfigureResource(resourceBuilder => resourceBuilder
      .AddService("MyServiceName", "MyServiceNamespace")
)
```
Hint: You can also add attributes, like environment, to the resource.


## Open Telemetry Exporters
By using the Open Telemetry Exportr extensions you have more control over the configuration. Fx what auto instrumentation should be used.

```csharp 
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
               TenantId = "c39886aa-7273-4937-9cad-53b86940713f"
           });
});
```

The `BifrostOptions` can also be used in case you want to use the same configuration for both logs, traces and metrics.

```csharp
var bifrostOptions = new BifrostOptions
{
    Endpoint = "https://your.bifrost.endpoint/otlp/http/v1",
    BifrostEnvironmentId = "d9a8719a-8bc2-4829-a078-231df13fd125",
    IdentityOptions = new MicrosoftIdentityOptions
        {
            Instance = "https://login.microsoftonline.com/",
            ClientId = "8be5cb8b-3a8d-47bf-9b70-660963b311ef",
            ClientSecret = "ThisIsYourClientSecret",
            TenantId = "c39886aa-7273-4937-9cad-53b86940713f"
        }
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

# How to Contribute
## Branching Strategy
Trunk based branching strategy is used. New features are added by creating feature branches that are merged to main with a pull request.
Pull requests requires the build pipeline to pass.

## Versioning
The nuget package follows [semver.org](https://www.semver.org).

## Release Procedure
These are the steps needed to create a new release:
1. Make sure the `CHANGELOG.md`is up to date in tha main branch.
2. In GitHub, create a new release.
    1. The tag version should be the same as the version in the `CHANGELOG.md` file, prefixed with a 'v'. For example `v1.2.3`.
    2. The release title should be the version number. Fx `1.2.3`. The release title is used as the version number in the nuget package.
3. The release pipeline will now create a new nuget package and publish it to nuget.org.

To build and publish the nuget package manually, do the following:
1. Build and test the solution `dotnet build` and `dotnet test`
2. Package the nuget package with the right version: `dotnet pack NovoNordisk.OpenTelemetry.Exporter.Bifrost -c Release /p:PackageVersion=[SEMVER. Fx 1.2.3]`

# TODO
- We could probably use BifrostOptions in the private methods of BifrostExporter and make the arguments simpler.
- Maybe we could use the config or environment variables in the `OpenTelemetryBuilderExtensions` so the user does not 
have to pass `BifrostOptions` as an argument.
- Add tests
