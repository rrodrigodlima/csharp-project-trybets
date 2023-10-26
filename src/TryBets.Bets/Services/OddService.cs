using System.Net.Http;
namespace TryBets.Bets.Services;

public class OddService : IOddService
{
    private readonly HttpClient _client;
    public OddService(HttpClient client)
    {
        _client = client;
    }

    public async Task<object> UpdateOdd(int MatchId, int TeamId, decimal BetValue)
    {
        Uri uri = new($"https://localhost:5504/odd/{MatchId}/{TeamId}/{BetValue}");
        HttpResponseMessage response = await _client.PatchAsync(uri.ToString(), null);
        string content = await response.Content.ReadAsStringAsync();

        return content;
    }
}