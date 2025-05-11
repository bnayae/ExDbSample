using Core.Abstractions;
using EvDb.Core;
using Funds.Abstractions;
using Funds.Withdraw.RequestWithdrawFundsViaATM;
using Microsoft.Extensions;
using Vogen;
[assembly: VogenDefaults(openApiSchemaCustomizations: OpenApiSchemaCustomizations.GenerateSwashbuckleSchemaFilter)]

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

IServiceCollection services = builder.Services;

#region Common Code

services.AddAuthentication();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(o => o.SchemaFilter<VogenSchemaFilterInFunds_Withdraw_Deployment>());

#endregion //  Common Code


services.AddRequestWithdrawFundsViaATM()
        .AddWithdrawalApprovalConsumer();

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


await app.RunAsync();
