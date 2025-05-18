using EvDb.Core;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Outbox2SQSViaSNS;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

IServiceCollection services = builder.Services;

#region Common Code

services.AddAuthentication();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

#endregion //  Common Code

#region OTEL

services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            .SetResourceBuilder(ResourceBuilder.CreateDefault()
                                               .AddService("Funds.Withdraw"))
            .AddEvDbInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddAWSInstrumentation() // optional but useful for AWS-specific tracing
            .AddSource("MongoDB.Driver.Core.Extensions.DiagnosticSources")
            .AddOtlpExporter()
            .AddOtlpExporter(m => m.Endpoint = new Uri("http://127.0.0.1:18889"));
    })
    .WithMetrics(meterProviderBuilder =>
    {
        meterProviderBuilder
            .SetResourceBuilder(ResourceBuilder.CreateDefault()
                                               .AddService("Funds.Withdraw"))
            .AddEvDbInstrumentation()
            .AddRuntimeInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddAWSInstrumentation()
            .AddMeter("MongoDB.Driver.Core.Extensions.DiagnosticSources")
            .AddOtlpExporter()
            .AddOtlpExporter(m => m.Endpoint = new Uri("http://127.0.0.1:18889"));
    });

#endregion //  OTEL

services.AddHostedService<Job>();

WebApplication app = builder.Build();

#region Common Code

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

#endregion //  Common Code


await app.RunAsync();
