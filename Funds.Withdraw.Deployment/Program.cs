using Funds.Withdraw;
using Microsoft.Extensions;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Vogen;
[assembly: VogenDefaults(openApiSchemaCustomizations: OpenApiSchemaCustomizations.GenerateSwashbuckleSchemaFilter)]

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

IServiceCollection services = builder.Services;

#region Common Code

services.AddAuthentication();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(o => o.SchemaFilter<VogenSchemaFilterInFunds_Withdraw_Deployment>());

#endregion //  Common Code


services.AddTypedClients();

services.AddRequestWithdrawFundsViaATM()
        .AddWithdrawalApprovalConsumer()
        .AddCommissionCalculationConsumer();

builder.AddRequestWithdrawFundsViaATMSwimlanes()
        .AddWithdrawFundsSwimlanes();

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