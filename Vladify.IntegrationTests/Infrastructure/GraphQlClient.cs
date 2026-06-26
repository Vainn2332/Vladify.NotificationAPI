using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Vladify.IntegrationTests.Constants;
using Vladify.IntegrationTests.GraphQL;

namespace Vladify.IntegrationTests.Infrastructure;

public class GraphQlClient
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _serializerOptions;

    public GraphQlClient(HttpClient client)
    {
        _client = client;
        _serializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public async Task<GraphQlResponse<TResponse>> SendAsync<TResponse>(string graphQlQuery, string? userEmail = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, TestConstants.GraphQlRoute)
        {
            Content = JsonContent.Create(new { query = graphQlQuery })
        };

        if (userEmail is not null)
        {
            var token = JwtTokenBuilder.GenerateTestJWT(userEmail);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        }

        using var response = await _client.SendAsync(request);

        return (await response.Content.ReadFromJsonAsync<GraphQlResponse<TResponse>>(_serializerOptions))!;
    }
}