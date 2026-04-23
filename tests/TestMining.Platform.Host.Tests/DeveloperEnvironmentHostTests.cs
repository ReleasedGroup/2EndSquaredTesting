using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace TestMining.Platform.Host.Tests;

public sealed class DeveloperEnvironmentHostTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> factory;

    public DeveloperEnvironmentHostTests(WebApplicationFactory<Program> factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task Root_renders_local_environment_shell()
    {
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/");
        response.EnsureSuccessStatusCode();

        var html = await response.Content.ReadAsStringAsync();

        Assert.Contains("<!DOCTYPE html>", html, StringComparison.Ordinal);
        Assert.Contains("blazor.web.js", html, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Status_endpoint_returns_dependency_snapshot()
    {
        using var client = factory.CreateClient();

        var payload = await client.GetFromJsonAsync<DeveloperEnvironmentStatusResponse>("/api/developer-environment/status");

        Assert.NotNull(payload);
        Assert.False(string.IsNullOrWhiteSpace(payload.EnvironmentName));
        Assert.True(payload.ArtefactStorage.Configured);
        Assert.False(payload.Postgres.Ready);
        Assert.True(payload.Fixtures.Configured);
    }

    public sealed record DeveloperEnvironmentStatusResponse(
        string EnvironmentName,
        string ArtefactStoragePath,
        DependencyCheckResponse ArtefactStorage,
        DependencyCheckResponse Postgres,
        DependencyCheckResponse Fixtures,
        DateTimeOffset CheckedAt);

    public sealed record DependencyCheckResponse(
        bool Configured,
        bool Ready,
        string Summary);
}
