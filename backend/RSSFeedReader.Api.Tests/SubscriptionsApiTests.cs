using System.Net;
using System.Net.Http.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using RSSFeedReader.Api.Contracts;

namespace RSSFeedReader.Api.Tests;

public sealed class SubscriptionsApiTests
{
    [Fact]
    public async Task Get_returns_empty_list_for_a_fresh_host()
    {
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var subscriptions = await client.GetFromJsonAsync<SubscriptionResponse[]>("/api/subscriptions");

        Assert.NotNull(subscriptions);
        Assert.Empty(subscriptions);
    }

    [Fact]
    public async Task Post_returns_entry_and_get_returns_ordered_entries_including_duplicates()
    {
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();
        const string firstUrl = "https://example.com/feed.xml";

        var firstResponse = await client.PostAsJsonAsync(
            "/api/subscriptions",
            new CreateSubscriptionRequest { Url = firstUrl });
        var secondResponse = await client.PostAsJsonAsync(
            "/api/subscriptions",
            new CreateSubscriptionRequest { Url = "not a URL" });
        var duplicateResponse = await client.PostAsJsonAsync(
            "/api/subscriptions",
            new CreateSubscriptionRequest { Url = firstUrl });

        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, secondResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, duplicateResponse.StatusCode);
        Assert.Equal(firstUrl, (await firstResponse.Content.ReadFromJsonAsync<SubscriptionResponse>())?.Url);

        var subscriptions = await client.GetFromJsonAsync<SubscriptionResponse[]>("/api/subscriptions");

        Assert.NotNull(subscriptions);
        Assert.Equal(
            [firstUrl, "not a URL", firstUrl],
            subscriptions.Select(subscription => subscription.Url));
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("{\"url\":42}")]
    [InlineData("{\"url\":null}")]
    public async Task Post_returns_bad_request_when_url_is_missing_or_not_a_string(string json)
    {
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("/api/subscriptions", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Cors_allows_the_local_ui_origin()
    {
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Options, "/api/subscriptions");
        request.Headers.Add("Origin", "http://localhost:5213");
        request.Headers.Add("Access-Control-Request-Method", "POST");

        var response = await client.SendAsync(request);

        Assert.Equal("http://localhost:5213", response.Headers.GetValues("Access-Control-Allow-Origin").Single());
    }
}
