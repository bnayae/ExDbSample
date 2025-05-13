using Amazon.Runtime;
using Funds.Abstractions;
using Funds.Withdraw.WithdrawFunds;
using System.Collections.Immutable;

namespace Microsoft.Extensions;

public static class TypedClientsExtensions
{
    public static void AddTypedClients(this IServiceCollection services)
    {
        services.AddHttpClient<IFundsSegmentation, FundsSegmentationClient>()
                                .SetHandlerLifetime(TimeSpan.FromMinutes(1));
        services.AddHttpClient<IFundsCommissionPolicy, FundsCommissionPolicyClient>()
                                .SetHandlerLifetime(TimeSpan.FromMinutes(1));
    }

    private sealed class FundsSegmentationClient(HttpClient _httpClient) : IFundsSegmentation
    {
        async Task<ImmutableArray<Segment>> IFundsSegmentation.GetSegmentsAsync(AccountId accountId,
                                                                                CancellationToken cancellationToken)
        {
            ImmutableArray<Segment> result = await _httpClient.GetFromJsonAsync<ImmutableArray<Segment>>($"segments/{accountId}",
                                                                                                         cancellationToken);
            return result;
        }
    }

    private sealed class FundsCommissionPolicyClient(HttpClient _httpClient) : IFundsCommissionPolicy
    {
        async Task<Commission> IFundsCommissionPolicy.GetCommissionAsync(ImmutableArray<Segment> segments,
                                                                         CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsJsonAsync<ImmutableArray<Segment>>($"commission-policy",
                                                                                   segments,
                                                                                   cancellationToken);
            response.EnsureSuccessStatusCode();

            Commission result = await response.Content
                                              .ReadFromJsonAsync<Commission>(cancellationToken);
            return result;
        }
    }
}
