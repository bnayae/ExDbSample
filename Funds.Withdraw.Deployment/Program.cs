using Funds.Abstractions;
using Funds.Withdraw.ATM;
using Microsoft.Extensions;
using Vogen;
[assembly: VogenDefaults(openApiSchemaCustomizations: OpenApiSchemaCustomizations.GenerateSwashbuckleSchemaFilter)]

const string MONGO_CONNECTION_KEY = "EvDbMongoDBConnection";

var builder = WebApplication.CreateBuilder(args);

IServiceCollection services = builder.Services;

services.AddAuthentication();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(o => o.SchemaFilter<VogenSchemaFilterInFunds_Withdraw_Deployment>());

services.AddEvDb()
        .AddAtmFundsWithdrawFactory(storage => storage.UseMongoDBStoreForEvDbStream(MONGO_CONNECTION_KEY),
                                    new EvDbStorageContext("funds", 
                                                                builder.Environment.EnvironmentName,
                                                                "withdraw"))
        .DefaultSnapshotConfiguration(storage => storage.UseMongoDBForEvDbSnapshot(MONGO_CONNECTION_KEY));   

services.AddFetchFundsSlice();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var withdraw = app.MapGroup("withdraw")
    .WithTags("Withdraw");

withdraw.MapPost("atm/{account}", async (Guid account, FundsTransactionData data, IFetchFundsFromAtm slice) =>
    {
        FetchFundsFromAtmRequest request = new(account, data);
        await slice.ProcessAsync(request);
        return Results.Ok();
    });


await app.RunAsync();
