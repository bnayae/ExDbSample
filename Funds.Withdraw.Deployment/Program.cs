using EvDb.Core;
using Funds.Withdraw;
using Microsoft.Extensions;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Vogen;

[assembly: VogenDefaults(openApiSchemaCustomizations: OpenApiSchemaCustomizations.GenerateSwashbuckleSchemaFilter)]

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

IServiceCollection services = builder.Services;

#region Common Code

services.AddAuthentication();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(o => o.SchemaFilter<VogenSchemaFilterInFunds_Withdraw_Deployment>());

#endregion //  Common Code

#region services.ConfigureHttpClientDefaults(...)

services.ConfigureHttpClientDefaults(b =>
{
    b.ConfigureHttpClient(http =>
    {
        // Set the base address and timeout for the HTTP client
        http.BaseAddress = new Uri($"http://localhost:{WireMockBootstrap.PORT}");
        // http.Timeout = TimeSpan.FromSeconds(30);

    });
});

#endregion //  services.ConfigureHttpClientDefaults(...)

services.AddTypedClients();

services.AddRequestWithdrawFundsViaATM()
        .AddWithdrawalApprovalConsumer()
        .AddCommissionCalculationConsumer();

builder.AddRequestWithdrawFundsViaATMSwimlanes()
        .AddWithdrawFundsSwimlanes();

#region OTEL

services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            .SetResourceBuilder(ResourceBuilder.CreateDefault()
                                               .AddService("Funds.Withdraw"))
            .AddEvDbInstrumentation()
            .AddHttpClientInstrumentation()
            .AddSource("YourActivitySourceName") // optional if you use ActivitySource
            .AddAWSInstrumentation() // optional but useful for AWS-specific tracing
            .AddSource("MongoDB.Driver.Core.Extensions.DiagnosticSources")
            .AddOtlpExporter();
    })
    .WithMetrics(meterProviderBuilder =>
    {
        meterProviderBuilder
            .SetResourceBuilder(ResourceBuilder.CreateDefault()
                                               .AddService("Funds.Withdraw"))
            .AddEvDbInstrumentation()
            .AddRuntimeInstrumentation()
            .AddHttpClientInstrumentation()
            .AddAWSInstrumentation()
            .AddMeter("MongoDB.Driver.Core.Extensions.DiagnosticSources")
            .AddOtlpExporter();
    });

#endregion //  OTEL

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

app.UseRequestWithdrawFundsViaATM();

// Start mock servers
using var cts = new CancellationTokenSource();
WireMockBootstrap.StartWireMock(cts.Token);

await app.RunAsync();

await cts.CancelAsync();