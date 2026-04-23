using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Headers;

namespace TestMining.Platform.Host.Tests.Integration;

public sealed class ShellHostTests : IClassFixture<ShellHostApplicationFactory>
{
    private readonly ShellHostApplicationFactory _factory;

    public ShellHostTests(ShellHostApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Dashboard_WhenAnonymous_ShouldRenderSignInPrompt()
    {
        using var client = CreateClient();

        var response = await client.GetAsync("/dashboard");
        var html = await response.Content.ReadAsStringAsync();

        Assert.True(response.IsSuccessStatusCode);
        Assert.Contains("Choose a local role", html, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ViewerShell_ShouldHideAuthoringAndAdministrationLinks()
    {
        using var client = CreateClient();

        await SignInAsync(client, "viewer");
        var response = await client.GetAsync("/dashboard");
        var html = await response.Content.ReadAsStringAsync();

        Assert.True(response.IsSuccessStatusCode);
        Assert.Contains("Dashboard", html, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("href=\"/scenarios\"", html, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("href=\"/recordings\"", html, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("href=\"/administration\"", html, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ViewerAdministrationRequest_ShouldRenderAccessDenied()
    {
        using var client = CreateClient();

        await SignInAsync(client, "viewer");
        var response = await client.GetAsync("/administration");
        var html = await response.Content.ReadAsStringAsync();

        Assert.True(response.IsSuccessStatusCode);
        Assert.Contains("Access denied", html, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AdministratorShell_ShouldExposeAdministrationWorkspace()
    {
        using var client = CreateClient();

        await SignInAsync(client, "administrator");
        var response = await client.GetAsync("/administration");
        var html = await response.Content.ReadAsStringAsync();

        Assert.True(response.IsSuccessStatusCode);
        Assert.Contains("Administration", html, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("policy controls", html, StringComparison.OrdinalIgnoreCase);
    }

    private HttpClient CreateClient()
    {
        return _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = true,
            BaseAddress = new Uri("https://localhost")
        });
    }

    private static async Task SignInAsync(HttpClient client, string userId)
    {
        using var content = new FormUrlEncodedContent(
        [
            new KeyValuePair<string, string>("userId", userId),
            new KeyValuePair<string, string>("returnUrl", "/dashboard")
        ]);

        using var request = new HttpRequestMessage(HttpMethod.Post, "/auth/local-sign-in")
        {
            Content = content
        };
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));

        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
}
