using Funds.Abstractions;
using Funds.Withdraw.RequestWithdrawFundsViaATM;
using Microsoft.Extensions;
using System.Xml.Linq;
using Vogen;
[assembly: VogenDefaults(openApiSchemaCustomizations: OpenApiSchemaCustomizations.GenerateSwashbuckleSchemaFilter)]

const string MONGO_CONNECTION_KEY = "EvDbMongoDBConnection";
const string DATABASE = "funds.withdraw";

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

IServiceCollection services = builder.Services;

services.AddAuthentication();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(o => o.SchemaFilter<VogenSchemaFilterInFunds_Withdraw_Deployment>());



#region EvDb

services.AddEvDb()
        .AddAtmFundsWithdrawFactory(storage => storage.UseMongoDBStoreForEvDbStream(MONGO_CONNECTION_KEY),
                                    new EvDbStorageContext(DATABASE,
                                                                builder.Environment.EnvironmentName))
        .DefaultSnapshotConfiguration(storage => storage.UseMongoDBForEvDbSnapshot(MONGO_CONNECTION_KEY));

#endregion //  EvDb

services.AddFetchFundsSlice()
        .AddWithdrawalApprovalSlice();


WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.AddRequestWithdrawFundsViaATM();


await app.RunAsync();
