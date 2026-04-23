using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TestMining.Platform.Fixtures;

namespace TestMining.Platform.Fixtures.Tests;

public sealed class FixtureHarnessTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> factory;

    public FixtureHarnessTests(WebApplicationFactory<Program> factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task Reset_restores_seeded_customer_data()
    {
        using var client = factory.CreateClient();

        var createdState = await CreateCustomerAsync(client, "Dorothy Vaughan", "dorothy@example.test", "Operator");
        Assert.Equal(4, createdState.Customers.Count);

        using var resetResponse = await client.PostAsync("/api/test/reset", content: null);
        resetResponse.EnsureSuccessStatusCode();

        var resetState = await client.GetFromJsonAsync<FixtureStateSnapshot>("/api/test/state");

        Assert.NotNull(resetState);
        Assert.Equal(3, resetState.Customers.Count);
        Assert.Contains(resetState.Customers, customer => customer.Name == "Ada Lovelace");
        Assert.Contains(resetState.AuditTrail, entry => entry.Contains("Seeded fixture data set.", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Routes_and_health_endpoint_are_available()
    {
        using var client = factory.CreateClient();

        using var healthResponse = await client.GetAsync("/healthz");
        healthResponse.EnsureSuccessStatusCode();

        var html = await client.GetStringAsync("/forms");

        Assert.Contains("Basic form with dynamic identifiers", html);
    }

    private static async Task<FixtureStateSnapshot> CreateCustomerAsync(HttpClient client, string name, string email, string role)
    {
        using var response = await client.PostAsJsonAsync("/api/customers", new CreateCustomerRequest(name, email, role));
        response.EnsureSuccessStatusCode();

        var state = await response.Content.ReadFromJsonAsync<FixtureStateSnapshot>();
        Assert.NotNull(state);
        return state;
    }
}
