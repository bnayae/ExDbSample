using Funds.Withdraw.WithdrawFunds;
using System.Collections.Immutable;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Funds.Withdraw;

public static class WireMockBootstrap
{
    private const int PORT = 8080;

    public static void StartWireMock(CancellationToken cancellationToken)
    {
        // https://github.com/wiremock/WireMock.Net/wiki/WireMock-as-a-standalone-process#option-3--coding-yourself
        WireMockServer server = WireMockServer.Start(PORT);
        server
            .Given(Request.Create()
                          .WithPath(r => r.StartsWith("segments/", StringComparison.InvariantCultureIgnoreCase))
                          .UsingGet())
            .RespondWith(Response.Create()
                                 .WithStatusCode(200)
                                 .WithHeader("Content-Type", "application/json")
                                 .WithBodyAsJson(ImmutableArray.Create(["ICP", "VIP"])));
        server
            .Given(Request.Create()
                          .WithPath("commission-policy")
                          .UsingPost())
            .RespondWith(Response.Create()
                                 .WithStatusCode(200)
                                 .WithHeader("Content-Type", "application/json")
                                 .WithBodyAsJson(Commission.From(0.05)));

        cancellationToken.Register(() =>
        {
            server.Stop();
            server.Dispose();
        });
    }
}
